using Microsoft.Owin.Security.Cookies;
using Owin.Security.Providers.Google;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using CodeBud.ExternalLib;

[assembly: OwinStartup(typeof(Startup))]

namespace CodeBud.ExternalLib
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Account/Login")
            });

            app.UseGoogleAuthentication(new GoogleAuthenticationOptions
            {
                ClientId = "YOUR_GOOGLE_CLIENT_ID",
                ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET",
                CallbackPath = new PathString("/signin-google")
            });
        }

    }
}