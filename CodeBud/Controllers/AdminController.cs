using CodeBud.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;


namespace CodeBud.Controllers
{
    [RoleAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly SessionService.SessionService _sessionService = new SessionService.SessionService();

        public ActionResult Index()
        {
            var user = _sessionService.GetCurrentUser();
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;
            return View();
        }
    }
}