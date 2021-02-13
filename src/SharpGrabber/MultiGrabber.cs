using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Internal;
using DotNetTools.SharpGrabber.Internal.Grabbers;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Grabs from various sources using grabbers registered on it.
    /// Members of this class are thread-safe and can be used simultaneously.
    /// </summary>
    /// <remarks>
    /// A multi-grabber can grab from several URIs simultaneously because it creates grabbers using registered factories.
    /// </remarks>
    public class MultiGrabber
    {
        #region Fields
        private readonly HashSet<IFactory<IGrabber>> _grabberFactories = new HashSet<IFactory<IGrabber>>();
        #endregion

        #region Internal Methods
        /// <summary>
        /// Creates a grabber instance using the specified factory.
        /// May be overriden to modify grabber creation behavior. By default, it uses parameterless constructors.
        /// </summary>
        protected virtual IGrabber Make(IFactory<IGrabber> factory) => factory.Create();
        #endregion

        #region Methods
        /// <summary>
        /// Registers the specified <see cref="IGrabber"/> factory.
        /// </summary>
        public void Register(IFactory<IGrabber> factory)
        {
            lock (_grabberFactories)
                _grabberFactories.Add(factory);
        }

        /// <summary>
        /// Registers a simple factory for the specified <typeparamref name="TGrabber"/>.
        /// The registered factory creates instances of the specified type using its parameterless constructor.
        /// </summary>
        public void Register<TGrabber>() where TGrabber : IGrabber
            => Register(new TypeFactory<TGrabber, IGrabber>());

        /// <summary>
        /// Unregisters the specified factory if previously registered.
        /// </summary>
        public void Unregister(IFactory<IGrabber> factory)
        {
            lock (_grabberFactories)
                _grabberFactories.Remove(factory);
        }

        /// <summary>
        /// Tries to find all compatible grabbers with the specified URI.
        /// </summary>
        public virtual IGrabber[] GetGrabbers(Uri uri)
        {
            var result = new List<IGrabber>();

            lock (_grabberFactories)
            {
                foreach (var grabberFactory in _grabberFactories)
                {
                    var grabber = Make(grabberFactory);

                    if (grabber.Supports(uri))
                        result.Add(grabber);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Transparently grabs information from the specified URI using the registered grabbers.
        /// </summary>
        /// <remarks>
        /// This method will return the <see cref="GrabResult"/> of the very first successful grab while suppressing exceptions
        /// thrown by unsuccessful grab attempts. Throws the last exception thrown by the grabber tried last.
        /// <para>Status of the grab is unavailable when using this method because it might automatically make use of multiple
        /// compatible grabbers. To have status of the grab, use <see cref="GetGrabbers(Uri)"/> and grab manually instead.</para>
        /// </remarks>
        public async Task<GrabResult> Grab(Uri uri)
        {
            // get all compatible grabbers
            var grabbers = GetGrabbers(uri);

            // is URI unsupported?
            if (grabbers.Length == 0)
                throw new UnsupportedGrabException();

            // grab using the first compatible grabber
            Exception lastException = null;
            foreach (var grabber in grabbers)
            {
                try
                {
                    return await grabber.GrabAsync(uri);
                }
                catch (GrabException exception)
                {
                    lastException = exception;
                }
            }

            // throw the last exception
            if (lastException == null) // just to suppress compiler's possible NullPointerException warning.
                throw new InvalidOperationException();
            throw lastException;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates an instance of <see cref="MultiGrabber"/> with internal providers registered. 
        /// </summary>
        public static MultiGrabber CreateDefault()
        {
            var multiGrabber = new MultiGrabber();
            multiGrabber.Register<YouTubeGrabber>();
            multiGrabber.Register<VimeoGrabber>();
            multiGrabber.Register<HlsGrabber>();
            return multiGrabber;
        }
        #endregion
    }
}
