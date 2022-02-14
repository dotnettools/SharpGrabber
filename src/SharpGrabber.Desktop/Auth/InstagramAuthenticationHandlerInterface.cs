using Avalonia.Threading;
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

        public async Task<GrabberBasicCredentials> ProvideCredentialsAsync(GrabberAuthenticationRequest request)
        {
            GrabberBasicCredentials credentials = null;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                credentials = await _mainWindow.ShowBasicAuthDialog(string.Format("Authenticate {0}", request.Grabber.Name));
            });
            return credentials;
        }

        public async Task<string> GetTwoFactorVerificationCodeAsync(GrabberAuthenticationRequest request, IResult<InstaTwoFactorLoginInfo> twoFactorInfo,
            IResult<TwoFactorLoginSMS> smsResult)
        {
            string code = null;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                code = await _mainWindow.ShowTwoFactorAuthDialog(text: "Two Factor Code (leave empty if approved from phone)");
            });
            return code;
        }
    }
}
