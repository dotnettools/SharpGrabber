using DotNetTools.SharpGrabber.Auth;
using InstagramApiSharp.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// When implemented, provides methods to simulate user interactions when logging into Instagram.
    /// </summary>
    public interface IInstagramAuthenticationInterface
    {
        /// <summary>
        /// Provides the username and password of an Instagram account.
        /// </summary>
        Task<GrabberBasicCredentials> ProvideCredentialsAsync(GrabberAuthenticationRequest request);

        /// <summary>
        /// Provides the verification code received in the process of two factor login.
        /// </summary>
        Task<string> GetTwoFactorVerificationCodeAsync(GrabberAuthenticationRequest request, IResult<InstaTwoFactorLoginInfo> twoFactorInfo,
            IResult<TwoFactorLoginSMS> smsResult);
    }
}
