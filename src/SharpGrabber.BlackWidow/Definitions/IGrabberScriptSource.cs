using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Definitions
{
    /// <summary>
    /// Provides access to the source of a grabber script.
    /// </summary>
    public interface IGrabberScriptSource
    {
        /// <summary>
        /// Gets the source code of the grabber script.
        /// </summary>
        string GetSource();

        /// <summary>
        /// Gets the source code of the grabber script.
        /// </summary>
        Task<string> GetSourceAsync();
    }
}
