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
using InterestClubWebAPI.Services;
using Microsoft.AspNetCore.Mvc.Routing;

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
            try
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
                        var result = MinIOManager.UploadFile(file, club.Title).Result;
                        if (result == string.Empty)
                        {
                            return BadRequest("Ошибка при загрузке изображения");
                        }

                        // Формируем URL для изображения
                        string imageUrl = result;
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
            catch (Exception ex) 
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }

        }

            [Authorize]
        [HttpDelete("DeleteClub")]
        public IActionResult DeleteClub(string id)
        {
            try
            {
                Club? club = _db.Clubs.FirstOrDefault(club => club.Id.ToString() == id);

                if (club != null)
                {
                    //// Путь к папке клуба
                    //string clubDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "MyStaticFiles/Clubs", club.Title);

                    //// Проверка, существует ли папка
                    //if (Directory.Exists(clubDirectoryPath))
                    //{
                    //    // Удаление папки и ее содержимого
                    //    Directory.Delete(clubDirectoryPath, true);
                    //}

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
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {
            try
            {
                Club? club = _db.Clubs.Include(c => c.Users).Include(c => c.Events).Include(c => c.Discussions).Include(c => c.ClubImage).FirstOrDefault(club => club.Id.ToString() == id);
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
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }

        }
        [AllowAnonymous]
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
       


        [Authorize]
        [HttpPost("EditClub")]
        public async Task<IActionResult> EditUser(string clubId,string title, string description, string fullDescription, IFormFile? file)
        {
            try
            {
                var token = HttpContext.GetTokenAsync("access_token");
                var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
                User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
                if (user == null)
                {
                    return BadRequest("Нет такого пользователя от которого идет запрос :(");
                }
                Club? club = _db.Clubs.Include(c => c.ClubImage).FirstOrDefault(c => c.Id.ToString() == clubId);
                if (club == null)
                {
                    return BadRequest("Нет такого клуба :(");
                }
                if (club.CreatorClubID == user.Id || user.Role == Enums.Role.admin)
                {
                    var result = MinIOManager.UploadFile(file, club.Title).Result;
                    if (result == string.Empty)
                    {
                        return BadRequest("Ошибка при загрузке изображения");
                    }

                    // Формируем URL для изображения
                    string imageUrl = result;
                    Image image = new Image { ImageName = file.FileName, Path = imageUrl };

                    club.Title = title;
                    club.Description = description;
                    club.FullDescription = fullDescription;
                    club.ClubImage = image;
                    _db.Images.Add(image);
                    _db.SaveChanges();
                    return Ok("Клуб успешно изменен");
                }
                else
                {
                    return BadRequest("Нет прав для изменения клуба :(");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
    }
}
