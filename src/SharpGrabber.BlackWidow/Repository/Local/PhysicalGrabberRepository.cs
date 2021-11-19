using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;
using Newtonsoft.Json;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository.Local
{
    /// <summary>
    /// Mounts to a phyisical directory to fetch and store grabbers.
    /// </summary>
    public class PhysicalGrabberRepository : GrabberRepositoryBase
    {
        private readonly string _rootPath;
        private readonly bool _readOnly;
        private bool _monitoring;

        public PhysicalGrabberRepository(string rootPath, bool readOnly = false)
        {
            _rootPath = rootPath;
            _readOnly = readOnly;
        }

        public override bool CanPut => !_readOnly;

        /// <summary>
        /// Gets or sets the descriptor file name without extension. The default value is 'descriptor.json'.
        /// </summary>
        public string DescriptorFileName { get; set; } = "descriptor.json";

        /// <summary>
        /// Gets or sets the script file name without extension. The default value is 'script'.
        /// </summary>
        public string ScriptFileNameWithoutExtension { get; set; } = "script";

        public override Task<IGrabberRepositoryFeed> GetFeedAsync(CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_rootPath);
            var root = new DirectoryInfo(_rootPath);
            var ids = root.EnumerateDirectories().Select(d => d.Name).ToArray();
            var scripts = ids.Select(id => ReadScriptInfo(id));
            var feed = new GrabberRepositoryFeed(scripts);
            return Task.FromResult<IGrabberRepositoryFeed>(feed);
        }

        public override Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script, CancellationToken cancellationToken)
        {
            var scriptPath = GetScriptPath(script);
            var source = new GrabberScriptSource(File.ReadAllText(scriptPath));
            return Task.FromResult<IGrabberScriptSource>(source);
        }

        public override async Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source, CancellationToken cancellationToken)
        {
            if (_readOnly)
                throw new NotSupportedException("The repository is read-only.");

            var descriptorPath = GetDescriptorPath(script.Id);
            var scriptPath = GetScriptPath(script);
            Directory.CreateDirectory(Path.GetDirectoryName(descriptorPath));
            Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));

            var sourceContent = await source.GetSourceAsync().ConfigureAwait(false);
            File.WriteAllText(descriptorPath, SerializeDescriptor(script));
            File.WriteAllText(scriptPath, sourceContent);

            if (_monitoring)
            {
                var newFeed = await GetFeedAsync(CancellationToken.None).ConfigureAwait(false);
                NotifyChanged(newFeed);
            }
        }

        protected override Task StartMonitoringAsync()
        {
            _monitoring = true;
            return Task.CompletedTask;
        }

        protected override Task StopMonitoringAsync()
        {
            _monitoring = false;
            return Task.CompletedTask;
        }

        protected virtual string SerializeDescriptor(IGrabberRepositoryScript script)
        {
            return JsonConvert.SerializeObject(script, Formatting.Indented);
        }

        protected virtual IGrabberRepositoryScript DeserializeDescriptor(string serializedValue)
        {
            return JsonConvert.DeserializeObject<GrabberRepositoryScript>(serializedValue);
        }

        private IGrabberRepositoryScript ReadScriptInfo(string scriptId)
        {
            var descriptorPath = GetDescriptorPath(scriptId);
            var fileContent = File.ReadAllText(descriptorPath);
            return DeserializeDescriptor(fileContent);
        }

        private string GetPath(IEnumerable<string> parts)
        {
            var array = new[] { _rootPath }.Union(parts).ToArray();
            return Path.Combine(array);
        }

        private string GetPath(params string[] parts)
            => GetPath((IEnumerable<string>)parts);

        private string GetDescriptorPath(string scriptId)
            => GetPath(scriptId, DescriptorFileName);

        private string GetScriptPath(IGrabberRepositoryScript script)
        {
            var attribute = script.Type.GetScriptTypeAttribute(false);
            var scriptName = string.IsNullOrEmpty(attribute?.FileExtension)
                ? ScriptFileNameWithoutExtension
                : $"{ScriptFileNameWithoutExtension}.{attribute.FileExtension}";
            return GetPath(script.Id, scriptName);
        }
    }
}
