using DotNetTools.SharpGrabber.Instagram;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Defines extension methods on <see cref="IGrabberAuthenticationService"/>.
    /// </summary>
    public static class InstagramAuthenticationHandlerExtensions
    {
        /// <summary>
        /// Registers the authentication handler for Instagram.
        /// </summary>
        public static IGrabberAuthenticationService RegisterInstagramHandler(this IGrabberAuthenticationService service,
            IInstagramAuthenticationInterface @interface)
        {
            service.RegisterHandler(new InstagramAuthenticationHandler(@interface, service.Store));
            return service;
        }

        /// <summary>
        /// Registers the authentication handler for Instagram.
        /// </summary>
        public static IGrabberAuthenticationService RegisterInstagramHandler<TInterface>(this IGrabberAuthenticationService service)
            where TInterface : IInstagramAuthenticationInterface
        {
            var @interface = Activator.CreateInstance<TInterface>();
            return service.RegisterInstagramHandler(@interface);
        }
    }
}
