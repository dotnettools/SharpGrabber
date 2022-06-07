using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetTools.SharpGrabber.Exceptions;
using Jint;

namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Accepts YouTube script source code and parses and executes the code.
    /// </summary>
    public class YouTubeScript
    {
        #region Internal Definitions
        /// <summary>
        /// Stores the starting and ending points of a scope. 
        /// </summary>
        private class ScopeAddress
        {
            /// <summary>
            /// Start point of function definition including variable name.
            /// </summary>
            public int StartPoint { get; set; }

            /// <summary>
            /// Ending point of the whole function definition
            /// </summary>
            public int EndPoint { get; set; }
        }

        /// <summary>
        /// Describes useful location points of function code in the script source. 
        /// </summary>
        private class FunctionAddress : ScopeAddress
        {
            /// <summary>
            /// Entry point of function body.
            /// </summary>
            public int EntryPoint { get; set; }
        }
        #endregion

        #region Fields
        private readonly string _source;
        private Engine _engine;
        #endregion

        #region Constructor

        public YouTubeScript(string scriptSource)
        {
            _source = scriptSource;
            _engine = new Engine();
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Tries to locate the next open brace and its matching close brace starting from the specified <paramref name="start"/>.
        /// Returns NULL if not found.
        /// </summary>
        private ScopeAddress LocateScopeBoundary(int start)
        {
            var address = new ScopeAddress();
            var scopeCount = 0;
            char? stringLiteral = null;

            for (var index = start; index < _source.Length; index++)
            {
                var ch = _source[index];
                switch (ch)
                {
                    case '{':
                        if (stringLiteral != null)
                            continue;
                        if (scopeCount == 0)
                            address.StartPoint = index;
                        scopeCount++;
                        break;

                    case '}':
                        if (stringLiteral != null)
                            continue;
                        scopeCount--;
                        if (scopeCount == 0)
                        {
                            address.EndPoint = index;
                            return address;
                        }

                        break;

                    case '"':
                    case '\'':
                        switch (stringLiteral)
                        {
                            case null:
                                // currently not in a string literal;
                                // assign current char as the new literal
                                stringLiteral = ch;
                                break;

                            default:
                                // currently in a string literal;
                                // terminate literal if it is the same character; otherwise ignore.
                                if (stringLiteral == ch)
                                    stringLiteral = null;
                                break;
                        }

                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to locate definition for the specified function.
        /// </summary>
        /// <returns>An instance of <see cref="ScopeAddress"/> describing function location or NULL if not found.</returns>
        private FunctionAddress LocateFunction(string functionName)
        {
            // locate starting point
            var regex = new Regex($@"{Regex.Escape(functionName)}=function\(", RegexOptions.Multiline);
            var match = regex.Match(_source);
            if (!match.Success)
                return null;

            // locate entry point and end point
            var address = LocateScopeBoundary(match.Index);
            if (address == null)
                throw new GrabParseException($"Failed to locate function boundary: {functionName}");

            return new FunctionAddress
            {
                StartPoint = match.Index,
                EntryPoint = address.StartPoint,
                EndPoint = address.EndPoint,
            };
        }

        /// <summary>
        /// Locates the specified function definition and returns its code.
        /// </summary>
        private string GetFunctionDefinition(string functionName)
        {
            var functionLocation = LocateFunction(functionName);
            var code = _source.Substring(functionLocation.StartPoint, functionLocation.EndPoint - functionLocation.StartPoint + 1);
            return code;
        }

        /// <summary>
        /// Extract function body from the given function definition.
        /// </summary>
        private string GetFunctionBody(string functionDefinition) => functionDefinition.Substring(functionDefinition.IndexOf('{'));

        /// <summary>
        /// Returns an array of every invoked function in the code.
        /// </summary>
        private string[] GetUsedFunctions(string code)
        {
            var set = new HashSet<string>();
            var jsonLocatorRegex = new Regex(@"([\w\$\.]+)\b\(", RegexOptions.Multiline);
            var matches = jsonLocatorRegex.Matches(code);
            foreach (Match match in matches)
                set.Add(match.Groups[1].Value);

            return set.ToArray();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Extracts and prepares part of the code that are required for the specified function to execute.
        /// This method is specialized for YouTube decipher method invocation.
        /// </summary>
        public void PrepareDecipherFunctionCall(string functionName)
        {
            // get function definition
            var functionDefinition = GetFunctionDefinition(functionName);
            var functionBody = GetFunctionBody(functionDefinition);

            // search for function container object
            string containerName = null;
            var methodBlackList = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
                {"split", "join"};
            foreach (var usage in GetUsedFunctions(functionBody))
            {
                var call = usage.Split(new[] { '.' }, 2);
                var targetObject = call.First();
                var targetMethod = call.Last();
                if (methodBlackList.Contains(targetMethod))
                    continue;
                containerName = targetObject;
                break;
            }

            if (string.IsNullOrEmpty(containerName))
                throw new GrabParseException("Failed to find decipher container object for helper functions.");

            // locate and extract container code
            var containerRegex = new Regex($@"var\s+{Regex.Escape(containerName)}\s*=\s*{{", RegexOptions.Multiline);
            var match = containerRegex.Match(_source);
            if (!match.Success)
                throw new GrabParseException("Failed to locate source code of container object for helper functions.");
            var containerScope = LocateScopeBoundary(match.Index)
                                 ?? throw new GrabParseException("Failed to determine boundaries of the container object for helper functions.");
            var containerBody = _source.Substring(match.Index, containerScope.EndPoint - match.Index + 1);

            // execute container and function on the engine
            _engine.Execute(containerBody);
            _engine.Execute(functionDefinition);
        }

        /// <summary>
        /// Calls the decipher function with the specified name supplied with the specified signature and returns the string result.
        /// </summary>
        public string CallDecipherFunction(string functionName, string signature)
        {
            return _engine.Invoke(functionName, signature).AsString();
        }
        #endregion
    }
}