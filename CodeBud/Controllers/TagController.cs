using CodeBud.Helpers;
using CodeBud.Models.Entities;
using MyProject.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeBud.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult Index()
        {
            GetUser();
            var db = AccountController._db;
            var tags = db.Tags.ToList();
            return View(tags); // Views/Tag/Index.cshtml
        }

        public ActionResult ByTag(int id)
        {
            GetUser();
            var db = AccountController._db;

            var questions = db.QuestionTags
                .Where(qt => qt.TagId == id)
                .Join(db.Questions, qt => qt.QuestionId, q => q.Id, (qt, q) => q)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();

            ViewBag.TagName = db.Tags.FirstOrDefault(t => t.TagId == id)?.TagName ?? "Etiket";

            return View("ByTag", questions);
        }
        public UserModel GetUser()
        {
            var user = JwtHelper.GetCurrentUserFromToken();
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;

            string virtualPath = string.IsNullOrEmpty(user.ImageURL) ? "~/Photos/default.jpg" : user.ImageURL;
            string physicalPath = Server.MapPath(virtualPath);

            ViewBag.ProfileImageUrl = System.IO.File.Exists(physicalPath)
                ? Url.Content(virtualPath)
                : "https://via.placeholder.com/150";

            return user;
        }
    }
}