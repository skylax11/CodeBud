using System.Linq;
using System.Web.Mvc;
using CodeBud.Models;
using CodeBud.SessionService;
using CodeBud.DbContext;
using CodeBud.Models.Entities;

namespace MyProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SessionService _sessionService = new SessionService();
        private readonly AppDbContext _db = new AppDbContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View(new UserModel());
        }

        [HttpPost]
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
        public ActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
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

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Logout()
        {
            _sessionService.ClearSession();
            return RedirectToAction("Login");
        }
    }
}
