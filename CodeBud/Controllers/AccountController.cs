
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using CodeBud.DbContext;
using CodeBud.Models.Entities;
using CodeBud.SessionService;
using System.Linq;

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
[ValidateAntiForgeryToken]
public ActionResult Register(UserModel model)
{
    if (ModelState.IsValid)
    {
        var existingUser = _db.Users.FirstOrDefault(u => u.Username == model.Username);
        if (existingUser != null)
        {
            ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
            return View();
        }

        model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        _db.Users.Add(model);
        _db.SaveChanges();
        return RedirectToAction("Login");
    }

    return View(model);
}

        [HttpGet]
public ActionResult Login()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Login(string username, string password)
{
    var user = _db.Users.FirstOrDefault(u => u.Username == username);
    if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
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
        // ✅ Google ile giriş
        public void LoginWithGoogle()
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
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = "/Account/GoogleCallback"
                    },
                    "Google"
                );

                Response.StatusCode = 401;
            }
        }

        // ✅ Google'dan döndükten sonra kullanıcıyı yakala
        public async Task<ActionResult> GoogleCallback()
        {
            var ctx = Request.GetOwinContext();
            var result = await ctx.Authentication.AuthenticateAsync("ExternalCookie");

            System.Diagnostics.Debug.WriteLine("Result null mu? => " + (result == null));
            System.Diagnostics.Debug.WriteLine("Identity null mu? => " + (result?.Identity == null));
            System.Diagnostics.Debug.WriteLine("Authenticated mi? => " + (result?.Identity?.IsAuthenticated ?? false));

            if (result?.Identity != null && result.Identity.IsAuthenticated)
            {
                var username = result.Identity.Name ?? "google_user";

                // Kullanıcı veritabanında var mı?
                var user = _db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    user = new UserModel
                    {
                        Username = username,
                        Password = null,
                        Role = "User"
                    };

                    _db.Users.Add(user);
                    _db.SaveChanges();
                }


                _sessionService.SetUserSession(user);
                return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "User");
            }

            return Content("❌ Kullanıcı doğrulanamadı.");
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
