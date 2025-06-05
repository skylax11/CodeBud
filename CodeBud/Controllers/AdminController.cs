using CodeBud.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using CodeBud.Helpers;
using CodeBud.DbContext;
using PagedList;
using CodeBud.Models.Entities;

namespace CodeBud.Controllers
{
    [RoleAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly SessionService.SessionService _sessionService = new SessionService.SessionService();

        public ActionResult Index()
        {
            var user = GetUser();

            using (var db = new AppDbContext())
            {
                var myQuestions = db.Questions
                                    .Where(q => q.UserId == user.Id)
                                    .OrderByDescending(q => q.CreatedAt)
                                    .Take(4)
                                    .ToList();

                var otherQuestions = db.Questions
                                       .Where(q => q.UserId != user.Id)
                                       .OrderByDescending(q => q.CreatedAt)
                                       .Take(4)
                                       .ToList();

                var userVotes = db.Votes
                                  .Where(v => v.UserId == user.Id && v.QuestionId != null)
                                  .ToList();

                ViewBag.MyQuestions = myQuestions;
                ViewBag.OtherQuestions = otherQuestions;
                ViewBag.UserVotes = userVotes;
            }

            return View();
        }
        public ActionResult ManageUsers(int? page)
        {
            GetUser();

            int pageSize = 5;
            int pageNumber = page ?? 1;

            

            using (var db = new AppDbContext())
            {
                var users = db.Users.OrderBy(u => u.Username).ToPagedList(pageNumber, pageSize);
                return View(users);
            }
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
        public ActionResult Edit(int id)
        {
            GetUser();

            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null)
                    return HttpNotFound();

                var currentUser = JwtHelper.GetCurrentUserFromToken();

                return View(user);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel updatedUser)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(updatedUser.Id);
                if (user == null)
                    return HttpNotFound();

                user.Username = updatedUser.Username;
                user.Role = updatedUser.Role;

                if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                {
                    // hashlemeni sen yapıyorsan burada yap
                    user.Password = updatedUser.Password;
                }

                db.SaveChanges();
                return RedirectToAction("Details","User", new { id = user.Id });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null)
                    return HttpNotFound();

                // 1. Kullanıcının yaptığı yorumlar
                var commentList = db.Comments.Where(x => x.UserId == id).ToList();
                foreach (var comment in commentList)
                {
                    db.Comments.Remove(comment);
                }

                // 2. Kullanıcının verdiği oylar
                var voteList = db.Votes.Where(x => x.UserId == id).ToList();
                foreach (var vote in voteList)
                {
                    var question = db.Questions.FirstOrDefault(x => x.Id == vote.QuestionId);
                    if (question != null)
                        question.VoteCount += vote.IsUpvote ? -1 : +1;

                    db.Votes.Remove(vote);
                    db.QuestionVote.Remove(db.QuestionVote.FirstOrDefault(x => x.VoteId == vote.Id));
                }

                // 3. Kullanıcının oluşturduğu postlar
                var userQuestions = db.Questions.Where(q => q.UserId == id).ToList();
                foreach (var post in userQuestions)
                {
                    var commentsOnPost = db.Comments.Where(c => c.QuestionId == post.Id).ToList();
                    foreach (var c in commentsOnPost)
                        db.Comments.Remove(c);

                    var votesOnPost = db.Votes.Where(v => v.QuestionId == post.Id).ToList();
                    foreach (var v in votesOnPost)
                    {
                        db.Votes.Remove(v);
                        db.QuestionVote.Remove(db.QuestionVote.FirstOrDefault(x => x.VoteId == v.Id));
                    }

                    db.QuestionTags.Remove(db.QuestionTags.FirstOrDefault(x => x.QuestionId == post.Id));
                    db.Questions.Remove(post);
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("ManageUsers");
        }



    }
}