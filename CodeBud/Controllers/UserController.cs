using CodeBud.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeBud.SessionService;
using System.IO;
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

            // Eğer kullanıcıda kayıtlı ImageURL varsa ve dosya varsa onu göster, yoksa placeholder
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

            if (profilePicture != null && profilePicture.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["UploadError"] = "Sadece JPG ve PNG dosyaları yükleyebilirsiniz.";
                    return RedirectToAction("ProfilePage");
                }

                // Profil fotoğrafı adı kullanıcı ID'sine göre verilir
                string fileName = $"{user.Id}{fileExtension}";
                string virtualPath = $"~/Photos/{fileName}";
                string physicalPath = Server.MapPath(virtualPath);

                // Fotoğrafı fiziksel olarak kaydet
                profilePicture.SaveAs(physicalPath);

                // Veritabanına SANAL yolu yaz
                var dbUser = AccountController._db.Users.FirstOrDefault(x => x.Id == user.Id);
                if (dbUser != null)
                {
                    dbUser.ImageURL = virtualPath;
                    AccountController._db.SaveChanges();
                }
            }

            return RedirectToAction("ProfilePage");
        }
    }
}
