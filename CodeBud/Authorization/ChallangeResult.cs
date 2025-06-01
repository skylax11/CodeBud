using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

public class ChallengeResult : HttpUnauthorizedResult
{
    public ChallengeResult(string provider, string redirectUri, string userId = null)
    {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
    }

    public string LoginProvider { get; set; }
    public string RedirectUri { get; set; }
    public string UserId { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        if (UserId != null)
        {
            properties.Dictionary["XsrfId"] = UserId;
        }

        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
    }
}
