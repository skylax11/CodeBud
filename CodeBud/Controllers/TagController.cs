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
            var db = AccountController._db;
            var tags = db.Tags.ToList();
            return View(tags); // Views/Tag/Index.cshtml
        }

        public ActionResult ByTag(int id)
        {
            var db = AccountController._db;

            var questions = db.QuestionTags
                .Where(qt => qt.TagId == id)
                .Join(db.Questions, qt => qt.QuestionId, q => q.Id, (qt, q) => q)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();

            ViewBag.TagName = db.Tags.FirstOrDefault(t => t.TagId == id)?.TagName ?? "Etiket";

            return View("ByTag", questions);
        }

    }
}