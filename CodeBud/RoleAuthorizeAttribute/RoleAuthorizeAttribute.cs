using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using System.Web.Routing;

namespace CodeBud.Web.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _requiredRoles;

        public RoleAuthorizeAttribute(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var sessionService = new SessionService.SessionService();
            var user = sessionService.GetCurrentUser();

            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Oturum yok. IP: {filterContext.HttpContext.Request.UserHostAddress}");

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }
            else if (!_requiredRoles.Contains(user.Role))
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Yetkisiz erişim: Kullanıcı rolü = {user.Role}, Gerekli roller = {string.Join(", ", _requiredRoles)}. IP: {filterContext.HttpContext.Request.UserHostAddress}");

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" })
                );
            }
        }
    }
}
