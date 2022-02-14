using DotNetTools.SharpGrabber.Auth;
using DotNetTools.SharpGrabber.Exceptions;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// Implements <see cref="IGrabberAuthenticationHandler"/> for Instagram.
    /// </summary>
    public class InstagramAuthenticationHandler : IGrabberAuthenticationHandler
    {
        private readonly IInstagramAuthenticationInterface _interface;
        private readonly IGrabberAuthenticationStore _store;

        public InstagramAuthenticationHandler(IInstagramAuthenticationInterface @interface, IGrabberAuthenticationStore store)
        {
            _interface = @interface ?? throw new ArgumentNullException(nameof(@interface));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public bool Supports(IGrabber grabber)
        {
            return grabber is InstagramGrabber;
        }

        public async Task<object> AuthenticateAsync(GrabberAuthenticationRequest request)
        {
            var state = request.GetRequestState<InstagramAuthenticationRequestState>()
                ?? throw new InvalidOperationException("Expected an Instagram request state object.");
            var api = state.Api;
            request.CancellationToken.ThrowIfCancellationRequested();

            // try use login state from the store
            var stateString = _store.Get(request);
            if (!string.IsNullOrEmpty(stateString))
            {
                await api.LoadStateDataFromStringAsync(stateString).ConfigureAwait(false);
                if (api.IsUserAuthenticated)
                    return stateString;
            }

            // login with credentials
            var credentials = await _interface.ProvideCredentialsAsync(request).ConfigureAwait(false);
            if (credentials == null)
                return null;
            api.SetUser(credentials.Username, credentials.Password);
            var loginResult = (await api.LoginAsync().ConfigureAwait(false)).Value;
            return await HandleLoginResultAsync(loginResult, api, request).ConfigureAwait(false);
        }

        private async Task<object> HandleLoginResultAsync(InstaLoginResult loginResult, IInstaApi api, GrabberAuthenticationRequest request)
        {
            switch (loginResult)
            {
                case InstaLoginResult.Success:
                    return await api.GetStateDataAsStringAsync().ConfigureAwait(false);

                case InstaLoginResult.TwoFactorRequired:
                    // perform two factor authentication
                    request.CancellationToken.ThrowIfCancellationRequested();
                    var twoFactorInfo = await api.GetTwoFactorInfoAsync().ConfigureAwait(false);
                    var smsResult = await api.SendTwoFactorLoginSMSAsync().ConfigureAwait(false);
                    var verificationCode = await _interface.GetTwoFactorVerificationCodeAsync(request, twoFactorInfo, smsResult).ConfigureAwait(false);
                    if (verificationCode == null)
                        return null;
                    if (verificationCode.Length == 0)
                    {
                        var success = false;
                        var approvedLoginResult = await api.LoginAsync(false).ConfigureAwait(false);
                        if (approvedLoginResult.Succeeded)
                        {
                            success = (await api.SendRequestsAfterLoginAsync().ConfigureAwait(false)).Value;
                        }
                        if (!success)
                            return null;
                        return await api.GetStateDataAsStringAsync().ConfigureAwait(false);
                    }
                    var twoFactorLoginResult = await api.TwoFactorLoginAsync(verificationCode).ConfigureAwait(false);
                    if (twoFactorLoginResult.Succeeded)
                        return await api.GetStateDataAsStringAsync().ConfigureAwait(false);
                    return await HandleTwoFactorResultAsync(twoFactorLoginResult.Value, api, request).ConfigureAwait(false);

                case InstaLoginResult.ChallengeRequired:
                    return await HandleChallengeAsync(api, request).ConfigureAwait(false);

                default:
                    throw new GrabAuthenticationException($"Instagram authentication error: {loginResult}");
            }
        }

        private async Task<object> HandleTwoFactorResultAsync(InstaLoginTwoFactorResult twoFactorLoginResult, IInstaApi api,
            GrabberAuthenticationRequest request)
        {
            return twoFactorLoginResult switch
            {
                InstaLoginTwoFactorResult.Success => await api.GetStateDataAsStringAsync().ConfigureAwait(false),
                InstaLoginTwoFactorResult.ChallengeRequired => await HandleChallengeAsync(api, request).ConfigureAwait(false),
                _ => throw new GrabAuthenticationException($"Instagram two factor authentication failed: {twoFactorLoginResult}"),
            };
        }

        private Task<object> HandleChallengeAsync(IInstaApi api, GrabberAuthenticationRequest request)
        {
            throw new NotImplementedException("Handling of Instagram login challenge is not implemented.");
        }

        public string SerializeState(object state)
        {
            return state as string ?? state?.ToString();
        }

        public object DeserializeState(string state)
        {
            return state;
        }
    }
}
