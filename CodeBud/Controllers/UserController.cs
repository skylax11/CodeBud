using CodeBud.Web.Filters;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using CodeBud.ExternalLib; // ← Fotoğraf servisi burada
using MyProject.Web.Controllers;

namespace CodeBud.Controllers
{
    [RoleAuthorize("User")]
    public class UserController : Controller
    {
        private readonly SessionService.SessionService _sessionService = new SessionService.SessionService();

        public ActionResult Index()
        {
            var user = _sessionService.GetCurrentUser();
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;
            return View();
        }

        public ActionResult ProfilePage()
        {
            var user = _sessionService.GetCurrentUser();
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;

            // Eğer kullanıcıda ImageURL varsa ve fiziksel dosya varsa onu göster
            string virtualPath = string.IsNullOrEmpty(user.ImageURL) ? "~/Photos/default.jpg" : user.ImageURL;
            string physicalPath = Server.MapPath(virtualPath);

            ViewBag.ProfileImageUrl = System.IO.File.Exists(physicalPath)
                ? Url.Content(virtualPath)
                : "https://via.placeholder.com/150";

            return View();
        }

        [HttpPost]
        public ActionResult UploadProfilePicture(HttpPostedFileBase profilePicture)
        {
            var user = _sessionService.GetCurrentUser();

            try
            {
                if (profilePicture != null && profilePicture.ContentLength > 0)
                {
                    // Servisle fotoğrafı yükle
                    var photoService = new PhotoUploadService(Server.MapPath("~"));
                    string relativePath = photoService.UploadProfilePhoto(profilePicture, user.Username); // dosya adı: username.jpg

                    // Veritabanına yolu kaydet
                    var dbUser = AccountController._db.Users.FirstOrDefault(x => x.Id == user.Id);
                    if (dbUser != null)
                    {
                        dbUser.ImageURL = "~" + relativePath; // dikkat: "~" ekliyoruz çünkü Server.MapPath böyle çalışır
                        AccountController._db.SaveChanges();

                        _sessionService.SetUserSession(dbUser);

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "Fotoğraf yüklenemedi: " + ex.Message;
            }

            return RedirectToAction("ProfilePage");
        }
    }
}
