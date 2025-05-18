using System.Threading.Tasks;
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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                _sessionService.SetUserSession(user);
                return RedirectToAction("Index", user.Role == "Admin" ? "Admin" : "User");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
            return View();
        }

        // ✅ Google ile giriş
        public void LoginWithGoogle()
        {
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
            return RedirectToAction("Login");
        }
    }
}
