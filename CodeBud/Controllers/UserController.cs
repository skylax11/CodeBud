using CodeBud.Web.Filters;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.ExternalLib; // ← Fotoğraf servisi burada
using MyProject.Web.Controllers;
using System.Threading.Tasks;
using CodeBud.DbContext;
using CodeBud.Helpers; // JWT Helper burada
using CodeBud.SessionService;
using CodeBud.Models.Entities;

namespace CodeBud.Controllers
{
    [RoleAuthorize("User")]
    public class UserController : Controller
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

        [HttpPost]
        public async Task<JsonResult> AskAI(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return Json(new { answer = "[Uyarı] Soru boş gönderildi." });
            }

            var llm = new LLMService();
            var response = await llm.AskLLMAsync(question);
            return Json(new { answer = response });
        }

        public ActionResult ProfilePage()
        {
            GetUser();

            return View();
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            var currentUser = GetUser();

            var user = AccountController._db.Users.FirstOrDefault(x => x.Id == currentUser.Id);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(UserModel updatedUser, HttpPostedFileBase profilePicture)
        {
            var currentUser = JwtHelper.GetCurrentUserFromToken();
            var dbUser = AccountController._db.Users.FirstOrDefault(x => x.Id == currentUser.Id);

            if (dbUser == null)
            {
                TempData["UploadError"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("ProfilePage");
            }

            // 🔐 Şifre boş değilse değiştir (güncellenmek isteniyor)
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                dbUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            // 📷 Profil resmi varsa yükle
            if (profilePicture != null && profilePicture.ContentLength > 0)
            {
                var photoService = new PhotoUploadService(Server.MapPath("~"));
                string relativePath = photoService.UploadProfilePhoto(profilePicture, dbUser.Username);
                dbUser.ImageURL = "~" + relativePath;
            }

            AccountController._db.SaveChanges();
            TempData["UploadSuccess"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("ProfilePage");
        }


        [HttpPost]
        public ActionResult UploadProfilePicture(HttpPostedFileBase profilePicture)
        {
            var user = JwtHelper.GetCurrentUserFromToken();

            try
            {
                if (profilePicture != null && profilePicture.ContentLength > 0)
                {
                    var photoService = new PhotoUploadService(Server.MapPath("~"));
                    string relativePath = photoService.UploadProfilePhoto(profilePicture, user.Username);

                    var dbUser = AccountController._db.Users.FirstOrDefault(x => x.Id == user.Id);
                    if (dbUser != null)
                    {
                        dbUser.ImageURL = "~" + relativePath;
                        AccountController._db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "Fotoğraf yüklenemedi: " + ex.Message;
            }

            return RedirectToAction("ProfilePage");
        }

        public ActionResult Details(int id)
        {
            ViewBag.User = GetUser();

            using (var db = new AppDbContext())
            {
                var targetUser = db.Users.FirstOrDefault(u => u.Id == id);
                if (targetUser == null)
                    return HttpNotFound();

                return View(targetUser);
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

        public ActionResult Logout()
        {
            Response.Cookies["jwt_token"].Expires = DateTime.Now.AddDays(-1); // JWT'yi sil
            return RedirectToAction("Login", "Account");
        }
    }
}
