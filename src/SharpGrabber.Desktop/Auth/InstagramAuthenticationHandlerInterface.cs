using DotNetTools.SharpGrabber.Auth;
using DotNetTools.SharpGrabber.Instagram;
using InstagramApiSharp.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrabber.Desktop.Auth
{
    internal class InstagramAuthenticationHandlerInterface : IInstagramAuthenticationInterface
    {
        private readonly MainWindow _mainWindow;

        public InstagramAuthenticationHandlerInterface(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public Task<GrabberBasicCredentials> ProvideCredentialsAsync(GrabberAuthenticationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTwoFactorVerificationCodeAsync(GrabberAuthenticationRequest request, IResult<InstaTwoFactorLoginInfo> twoFactorInfo,
            IResult<TwoFactorLoginSMS> smsResult)
        {
            throw new NotImplementedException();
        }
    }
}
