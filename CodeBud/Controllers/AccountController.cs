using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using CodeBud.DbContext;
using CodeBud.Models.Entities;
using CodeBud.SessionService;




namespace MyProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SessionService _sessionService = new SessionService();
        public static readonly AppDbContext _db = new AppDbContext();

        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

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
            _db.Database.CreateIfNotExists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public ActionResult Logout()
        {
            _sessionService.ClearSession();
            AuthManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            var loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Login");

            var email = loginInfo.Email;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new UserModel
                {
                    Username = loginInfo.DefaultUserName ?? email.Split('@')[0],
                    Email = email,
                    Role = "User",
                    HashedPassword = "ExternalLogin"
                };
                _db.Users.Add(user);
                _db.SaveChanges();
            }

            _sessionService.SetUserSession(user);

            return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "User");
        }
    }
}
