using System;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using CodeBud.Models;

namespace CodeBud.Web.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var sessionService = new SessionService.SessionService();
            var user = sessionService.GetCurrentUser();

            if (user == null || user.Role != _requiredRole)
            {
                // Access Denied sayfasına yönlendir
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Account", action = "AccessDenied" }
                    )
                );
            }
        }
    }
}
