using CodeBud.DbContext;
using CodeBud.Models.Entities;
using MyProject.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using PagedList;
using CodeBud.Helpers;

namespace CodeBud.Controllers
{
    public class QuestionController : Controller
    {
        private readonly SessionService.SessionService _sessionService = new SessionService.SessionService();

        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            using (var db = new AppDbContext())
            {
                var questions = db.Questions
                                  .Include("User")
                                  .OrderByDescending(q => q.CreatedAt)
                                  .ToPagedList(pageNumber, pageSize);

                var currentUser = JwtHelper.GetCurrentUserFromToken();

                ViewBag.CurrentUserId = currentUser?.Id;
                ViewBag.CurrentUserRole = currentUser?.Role;

                // 🔥 Kullanıcının verdiği oyları da çek
                var userVotes = db.Votes
                                  .Where(v => v.UserId == currentUser.Id && v.QuestionId != null)
                                  .ToList();
                ViewBag.UserVotes = userVotes;

                return View(questions);
            }
        }


        public ActionResult Create()
        {
            var db = AccountController._db; // 👈 dispose etme, zaten global
            
                ViewBag.Tags = db.Tags.ToList();
            

            return View(new Question());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Question model, string NewTag)
        {
            model.UserId = JwtHelper.GetCurrentUserFromToken().Id;
            model.CreatedAt = DateTime.Now;

            var db = AccountController._db; // 👈 dispose etme, zaten global

            if (!string.IsNullOrWhiteSpace(NewTag))
            {
                var existingTag = db.Tags.FirstOrDefault(t => t.TagName.ToLower() == NewTag.ToLower());

                if (existingTag != null)
                {
                    model.TagId = existingTag.TagId;
                }
                else
                {
                    var newTag = new Tag { TagName = NewTag };
                    db.Tags.Add(newTag);
                    db.SaveChanges();

                    model.TagId = newTag.TagId;
                }
            }

            db.Questions.Add(model);

            db.SaveChanges();

            db.QuestionTags.Add(new Models.Repository.Relations.QuestionTagMatch()
            {
                QuestionId = model.Id,
                TagId = model.TagId
            });

            db.SaveChanges();

            return RedirectToAction("Index", "User");
        }


        [HttpPost]
        [PermissionAuthorize("CanDeleteQuestion")]
        public ActionResult Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var question = db.Questions.FirstOrDefault(q => q.Id == id);
                if (question == null) return HttpNotFound();

                db.Questions.Remove(question);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        public ActionResult Details(int id)
        {
            using(var db = new AppDbContext())
            {
                var question = db.Questions
                                 .Include("User")             // Soran kullanıcıyı dahil et
                                 .Include("Comments.User")    // Yorumları ve her yorumun yazarını dahil et
                                 .FirstOrDefault(q => q.Id == id);

                if (question == null)
                    return HttpNotFound();

                return View(question);
            }

        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult AddComment(int questionId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                TempData["Error"] = "Yorum boş olamaz.";
                return RedirectToAction("Details", new { id = questionId });
            }

            using (var db = new AppDbContext())
            {
                var comment = new Comment
                {
                    Content = commentText,
                    CreatedAt = DateTime.Now,
                    QuestionId = questionId,
                    UserId = JwtHelper.GetCurrentUserFromToken().Id
                };

                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = questionId });
        }
    }
}