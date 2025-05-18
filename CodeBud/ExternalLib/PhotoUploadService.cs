using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CodeBud.ExternalLib
{
    public class PhotoUploadService
    {
        private readonly string _basePath;

        public PhotoUploadService(string serverMapPath)
        {
            _basePath = Path.Combine(serverMapPath, "Photos");

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public string UploadProfilePhoto(HttpPostedFileBase photo, string userName)
        {
            if (photo == null || photo.ContentLength == 0)
                throw new ArgumentException("Dosya boş.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(photo.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Geçersiz dosya türü.");

            // Dosya adı = kullanıcı adı + uzantı
            var fileName = userName + ext;
            var savePath = Path.Combine(_basePath, fileName);

            photo.SaveAs(savePath);

            // Geriye relative path döndür (veritabanına yazmak için)
            return "/Photos/" + fileName;
        }
    }
}