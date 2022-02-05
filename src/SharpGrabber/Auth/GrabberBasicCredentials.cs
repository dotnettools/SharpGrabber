using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Defines basic authentication credentials.
    /// </summary>
    public class GrabberBasicCredentials
    {
        public GrabberBasicCredentials()
        {
        }

        public GrabberBasicCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
