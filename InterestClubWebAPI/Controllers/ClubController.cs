using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.DTOs;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting.Server;

namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IJWTAuthManager _authentication;
        IWebHostEnvironment _appEnvironment;
        public ClubController(IJWTAuthManager authentication, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _authentication = authentication;
            _db = context;
            _appEnvironment = appEnvironment;
        }
        [Authorize]
        [HttpPost("CreateClub")]
        public async Task<IActionResult> CreateClub(string title, string description, string fullDescription, string userId, IFormFile file)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (_db.Clubs.Any(c => c.Title == title))
            {
                return BadRequest("Club with the same title already exists.");
            }
            Club club = new Club { Title = title, Description = description, FullDescription = fullDescription, CreatorClubID = user.Id };
            club.CreatorClubID = user.Id;
            club.Users.Add(user);
            _db.Clubs.Add(club);
            _db.SaveChanges();            
            if (file != null)
            {
                try
                {
                    string rezult;
                    rezult = await SaveFileModel.SaveFile(_appEnvironment.ContentRootPath, "Clubs", club.Title,file);
                    if (rezult != "Изображение успешно сохранено") 
                    {
                        return BadRequest(rezult);
                    }                    

                    // Формируем URL для изображения
                    string imageUrl = Url.Content("~/StaticFiles/Clubs/" + club.Title + "/" + file.FileName);
                    Image image = new Image { ImageName = file.FileName, Path = imageUrl };
                    club.ClubImage = image;
                    _db.Images.Add(image);
                    _db.SaveChanges();

                    //return Ok(new { message = "Изображение успешно добавлено", imageUrl });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Ошибка при загрузке файла", error = ex.Message });
                }

            }
            var clubDTO = club.ToDTO();

            return Ok(clubDTO);
        }

            [Authorize]
        [HttpDelete("DeleteClub")]
        public IActionResult DeleteClub(string id)
        {
            Club? club = _db.Clubs.FirstOrDefault(club => club.Id.ToString() == id);

            if (club != null)
            {
                // Путь к папке клуба
                string clubDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "MyStaticFiles\\Clubs", club.Title);

                // Проверка, существует ли папка
                if (Directory.Exists(clubDirectoryPath))
                {
                    // Удаление папки и ее содержимого
                    Directory.Delete(clubDirectoryPath, true);
                }

                // Удаление записи клуба из базы данных
                _db.Clubs.Remove(club);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Нет клуба с таким ID");
            }
        }


        [AllowAnonymous]
        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {

            Club? club = _db.Clubs.Include(c => c.Users).Include(c => c.Events).Include(c => c.Discussions).FirstOrDefault(club => club.Id.ToString() == id);
            if (club == null)
            {
                return BadRequest("Такого Клуба нет :(");
            }
            else
            {
                var clubDTO = club.ToDTO();
                return Ok(clubDTO);
            }
        }
        [AllowAnonymous]
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            var clubs = _db.Clubs.Include(c => c.Users).Include(c => c.Events).Include(c => c.Discussions).Include(c => c.ClubImage).ToList();

            if (clubs.Any())
            {
                List<ClubDTO> clubDTOs = new List<ClubDTO>();

                foreach (var club in clubs)
                {
                    var clubDTO = club.ToDTO();
                    clubDTOs.Add(clubDTO);
                }

                return Ok(clubDTOs);
            }
            else
            {
                return BadRequest("Такого Клуба нет :(");
            }
        }
       


        [Authorize]
        [HttpPost("EditClub")]
        public async Task<IActionResult> EditUser(string clubId,string title, string description, string fullDescription, IFormFile? file)
        {
            var token = HttpContext.GetTokenAsync("access_token");
            var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
            User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
            if(user == null)
            {
                return BadRequest("Нет такого пользователя от которого идет запрос :(");
            }
            Club? club = _db.Clubs.Include(c => c.ClubImage).FirstOrDefault(c => c.Id.ToString()== clubId);
            if(club == null)
            {
                return BadRequest("Нет такого клуба :(");
            }
            if(club.CreatorClubID == user.Id || user.Role == Enums.Role.admin) 
            {
                club.Title = title;
                club.Description = description;
                club.FullDescription = fullDescription;

                if (file != null) 
                {
                    // Удаление старого изображения, если оно существует
                    if (club.ClubImage != null)
                    {
                        string oldImagePath = _appEnvironment.ContentRootPath + club.ClubImage.Path;
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        _db.Images.Remove(club.ClubImage);
                    }
                    string rezult;
                    rezult = await SaveFileModel.SaveFile(_appEnvironment.ContentRootPath, "Clubs", club.Title, file);
                    if (rezult != "Изображение успешно сохранено")
                    {
                        return BadRequest(rezult);
                    }
                }
                _db.SaveChanges();                
                return Ok("Клуб успешно изменен");
            }
            else
            {
                return BadRequest("Нет прав для изменения клуба :(");
            }            

        }
    }
}
