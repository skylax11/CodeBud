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
                                  .OrderByDescending(q => q.CreatedAt)
                                  .ToPagedList(pageNumber, pageSize);
                return View(questions);
            }
        }
        public ActionResult Create()
        {
            return View(new Question());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Question model)
        {
            
            model.UserId = _sessionService.GetCurrentUser().Id;
            model.CreatedAt = DateTime.Now;

            using (AppDbContext db = AccountController._db)
            {
                db.Questions.Add(model);
                db.SaveChanges();
            }

            return RedirectToAction("UserPanel", "User");
            
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
                    UserId = _sessionService.GetCurrentUser().Id
                };

                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = questionId });
        }


    }
}