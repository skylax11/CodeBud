using System.Web;
using System.Web.Mvc;
using CodeBud.DbContext;
using CodeBud.Models.Entities;
using CodeBud.SessionService;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace MyProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SessionService _sessionService = new SessionService();
        public static readonly AppDbContext _db = new AppDbContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View(new UserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserModel model)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                return View();
            }

            model.HashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            _db.Users.Add(model);
            int turned = _db.SaveChanges();

            if (turned != 0)
                return RedirectToAction("Login");
            else
                return View(model);
        }
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
            {
                _sessionService.SetUserSession(user);
        
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "User");
            }
        
            ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
            return View();
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
