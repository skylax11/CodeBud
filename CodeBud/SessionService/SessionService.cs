using System;
using System.Web;
using CodeBud.Models.Entities;
using Newtonsoft.Json;

namespace CodeBud.SessionService
{
    public class SessionService
    {
        private readonly HttpContext _context;
        private const string UserSessionKey = "CurrentUser";
        private const string UserCookieKey = "UserCookie";

        public SessionService()
        {
            _context = HttpContext.Current;
        }

        public void SetUserSession(UserModel user)
        {
            var safeUser = new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                name = user.name,
                surName = user.surName
            };

            var sessionData = JsonConvert.SerializeObject(safeUser);
            _context.Session[UserSessionKey] = sessionData;

            var cookie = new HttpCookie(UserCookieKey, sessionData)
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true
            };
            _context.Response.Cookies.Add(cookie);
        }

        public UserModel GetCurrentUser()
        {
            var sessionData = _context.Session[UserSessionKey] as string;

            if (string.IsNullOrEmpty(sessionData))
            {
                var cookie = _context.Request.Cookies[UserCookieKey];
                sessionData = cookie?.Value;
            }

            return string.IsNullOrEmpty(sessionData)
                ? null
                : JsonConvert.DeserializeObject<UserModel>(sessionData);
        }

        public void ClearSession()
        {
            _context.Session.Remove(UserSessionKey);

            var expiredCookie = new HttpCookie(UserCookieKey)
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true
            };
            _context.Response.Cookies.Add(expiredCookie);
        }
    }
}
