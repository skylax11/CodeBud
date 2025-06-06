using System.Linq;
using System.Web.Mvc;
using CodeBud.Models.Entities;
using CodeBud.DbContext;
using CodeBud.SessionService;
using CodeBud.Helpers;

public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string[] _requiredPermissions;

    public PermissionAuthorizeAttribute(params string[] requiredPermissions)
    {
        _requiredPermissions = requiredPermissions;
    }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        var sessionService = new SessionService();
        var currentUser = JwtHelper.GetCurrentUserFromToken();

        if (currentUser == null)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
            return;
        }

        using (var db = new AppDbContext())
        {
            var userPermissions = db.UserPermissions
                .Where(up => up.UserId == currentUser.Id)
                .Select(up => up.Permission.Name)
                .ToList();

            bool hasPermission = _requiredPermissions.All(rp => userPermissions.Contains(rp));

            if (hasPermission)
                return;

            var routeData = filterContext.RouteData;
            string controller = routeData.Values["controller"].ToString().ToLower();
            string action = routeData.Values["action"].ToString().ToLower();

            if (controller == "question" && action == "delete")
            {
                int questionId;
                if (int.TryParse(filterContext.HttpContext.Request["id"], out questionId))
                {
                    var question = db.Questions.FirstOrDefault(q => q.Id == questionId);
                    if (question != null && question.UserId == currentUser.Id)
                        return;

                    if (currentUser.Role.ToLower() == "admin")
                        return;
                }
            }

            if (controller == "question" && action == "deletecomment")
            {
                int commentId;
                if (int.TryParse(filterContext.HttpContext.Request["id"], out commentId))
                {
                    var comment = db.Comments
                                    .Include("Question")
                                    .FirstOrDefault(c => c.Id == commentId);

                    if (comment != null)
                    {
                        if (comment.UserId == currentUser.Id) return;
                        if (comment.Question != null && comment.Question.UserId == currentUser.Id) return;
                        if (currentUser.Role.ToLower() == "admin") return;
                    }
                }
            }

            filterContext.Result = new RedirectResult("/Account/AccessDenied");
        }
    }
}
