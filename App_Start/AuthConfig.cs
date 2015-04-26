using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using youknow.Models;

namespace youknow
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            OAuthWebSecurity.RegisterTwitterClient(
                consumerKey: "KjzRP1C6THjzDF5di3WwNQ",
                consumerSecret: "zOnP7yBzfVNEmH1zwQWfDt0CfCfXEgIQqjOuD6B4");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");
            OAuthWebSecurity.RegisterFacebookClient(
               appId: "181949661990682",
               appSecret: "3dbd34370d70c0c25b77cb34035c376e");
            OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
