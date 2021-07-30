using Newtonsoft.Json;

namespace DotNetTools.SharpGrabber.Adult.Internal
{
    internal static class JsonHelper
    {
        /// <remarks>
        /// Adopted from https://stackoverflow.com/a/51428508/492352 with minor modifications
        /// </remarks>
        public static bool TryParseJson<T>(string text, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(text, settings);
            return success;
        }

        public static bool TryParseJson(string text, out object result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject(text, settings);
            return success;
        }
    }
}
