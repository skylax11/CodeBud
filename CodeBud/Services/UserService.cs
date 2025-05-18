using CodeBud.DbContext;
using CodeBud.Models.Entities;
using CodeBud.ExternalLib;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace CodeBud.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService()
        {
            _db = new AppDbContext();
        }

        // 📌 ID ile kullanıcıyı getir
        public UserModel GetById(int id)
        {
            return _db.Users.FirstOrDefault(u => u.Id == id);
        }

        // 📌 Kullanıcı adıyla getir (Login için veya kontrol amaçlı)
        public UserModel GetByUsername(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Username == username);
        }

        // 📌 Yeni kullanıcı oluştur
        public void CreateUser(UserModel newUser)
        {
            _db.Users.Add(newUser);
            _db.SaveChanges();
        }

        // 📌 Kullanıcı bilgilerini güncelle
        public void UpdateUser(UserModel updatedUser)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null) return;

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;
            user.Password = updatedUser.Password; // Şifre hash’li değilse ileride Hash eklenmeli
            _db.SaveChanges();
        }

        // 📌 Kullanıcıyı sil
        public void DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return;

            _db.Users.Remove(user);
            _db.SaveChanges();
        }

        // 📌 Profil fotoğrafı yükle
        public string UploadProfileImage(HttpPostedFileBase file, string username)
        {
            var photoService = new PhotoUploadService(HttpContext.Current.Server.MapPath("~"));
            return photoService.UploadProfilePhoto(file, username); // relative path döner
        }

        // 📌 Fotoğraf yolunu güncelle
        public void UpdateUserImagePath(int userId, string relativePath)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            user.ImageURL = "~" + relativePath;
            _db.SaveChanges();
        }

        // 📌 Tüm kullanıcıları getir
        public List<UserModel> GetAllUsers()
        {
            return _db.Users.ToList();
        }
    }
}
