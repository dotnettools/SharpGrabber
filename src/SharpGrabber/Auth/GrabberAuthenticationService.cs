using DotNetTools.SharpGrabber.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Auth
{
    internal class GrabberAuthenticationService : IGrabberAuthenticationService
    {
        private readonly HashSet<IGrabberAuthenticationHandler> _handlers = new();

        public GrabberAuthenticationService() : this(null)
        {
        }

        public GrabberAuthenticationService(IGrabberAuthenticationStore store)
        {
            Store = store ?? new InMemoryAuthenticationStore();
        }

        public IGrabberAuthenticationStore Store { get; }

        public IDisposable RegisterHandler(IGrabberAuthenticationHandler handler)
        {
            _handlers.Add(handler);
            return new RegisteredHandler(this, handler);
        }

        public async Task<GrabberAuthenticationResult> RequestAsync(GrabberAuthenticationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var grabber = request.Grabber;
            foreach (var handler in _handlers)
            {
                if (!handler.Supports(grabber))
                    continue;

                try
                {
                    var state = await handler.AuthenticateAsync(request).ConfigureAwait(false);
                    if (state == null)
                        throw new GrabAuthenticationException();
                    return new(state);
                }
                catch (Exception ex)
                {
                    if (ex is GrabAuthenticationException)
                        throw;
                    throw new GrabAuthenticationException("Failed to authenticate the grabber. See the inner exception for details.", ex);
                }
            }

            throw new GrabAuthenticationException($"No authentication handler is registered for {grabber.Name} grabber.");
        }

        private sealed class RegisteredHandler : IDisposable
        {
            private IGrabberAuthenticationHandler _handler;
            private GrabberAuthenticationService _self;

            public RegisteredHandler(GrabberAuthenticationService self, IGrabberAuthenticationHandler handler)
            {
                _self = self;
                _handler = handler;
            }

            public void Dispose()
            {
                _self._handlers.Remove(_handler);
            }
        }
    }
}
