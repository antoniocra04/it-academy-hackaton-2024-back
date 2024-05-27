using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Policy;

using Microsoft.EntityFrameworkCore;

namespace InterestClubWebAPI.Models
{
    public static class SaveFileModel
    {
        public async static Task<string> SaveFile(string ContentRootPath,string imageType,string title ,IFormFile file)
        {
            // Проверка, является ли файл изображением
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!permittedExtensions.Contains(ext))
            {
                return "Файл не является изображением";
            }

            // Проверка типа содержимого
            var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!permittedContentTypes.Contains(file.ContentType))
            {
                return "Файл не является изображением";
            }
            //// путь к папке Files
            string folderPath = Path.Combine(ContentRootPath, $"MyStaticFiles\\{imageType}", title);
            string filePath = Path.Combine(folderPath, file.FileName);
            //// Создание папки, если она не существует
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }            
            // сохраняем файл в папку Files в каталоге wwwroot
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }           

            return "Изображение успешно сохранено";
        }
    }
}
