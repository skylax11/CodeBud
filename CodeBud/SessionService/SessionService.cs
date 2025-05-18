using System.Web;
using CodeBud.Models;
using CodeBud.Models.Entities;
using Newtonsoft.Json;

namespace CodeBud.SessionService
{
    public class SessionService
    {
        private readonly HttpContext _context;

        public SessionService()
        {
            _context = HttpContext.Current;
        }

        private const string UserSessionKey = "CurrentUser";

        public void SetUserSession(UserModel user)
        {
            var sessionData = JsonConvert.SerializeObject(user);
            _context.Session[UserSessionKey] = sessionData;
        }

        public UserModel GetCurrentUser()
        {
            var sessionData = _context.Session[UserSessionKey] as string;
            return sessionData == null ? null : JsonConvert.DeserializeObject<UserModel>(sessionData);
        }

        public void ClearSession()
        {
            _context.Session.Remove(UserSessionKey);
        }
    }
}
