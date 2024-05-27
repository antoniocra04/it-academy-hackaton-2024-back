using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models.DTOs;
using static System.Reflection.Metadata.BlobBuilder;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
//using System.Data.Entity;


namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        private readonly IJWTAuthManager _authentication;
        private readonly ApplicationContext _db;
        IWebHostEnvironment _appEnvironment;

        public EventsController(IJWTAuthManager authentication, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _authentication = authentication;
            _db = context;
            _appEnvironment = appEnvironment;
        }
        [Authorize]
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent(string name, string description, string fullDescription, string idUser, string idClub, IFormFile file)
        {
            if (_db.Events.Any(e => e.Name == name))
            {
                return BadRequest("Event with the same name already exists.");
            }

            User? user = _db.Users.FirstOrDefault(u => u.Id.ToString() == idUser);
            Club? club = _db.Clubs.FirstOrDefault(c => c.Id.ToString() == idClub);

            if (user == null || club == null)
            {
                return BadRequest("User or Club not found.");
            }

            var eventDate = DateTime.Now;
            var ev = new Event { Name = name, Description = description, FullDescription = fullDescription, EventDate = eventDate.ToString() };//
            ev.CreatorEventID = user.Id;
            ev.ClubID = club.Id;
            ev.Members.Add(user);
            _db.Events.Add(ev);
            _db.SaveChanges();
            if (file != null)
            {
                try
                {
                    // Проверка, является ли файл изображением
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!permittedExtensions.Contains(ext))
                    {
                        return BadRequest("Файл не является изображением");
                    }

                    // Проверка типа содержимого
                    var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!permittedContentTypes.Contains(file.ContentType))
                    {
                        return BadRequest("Файл не является изображением");
                    }
                    // путь к папке Files
                    string folderPath = Path.Combine(_appEnvironment.ContentRootPath, "Files\\Events", ev.Name);
                    string filePath = Path.Combine(folderPath, file.FileName);
                    // Создание папки, если она не существует
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Удаление старого изображения, если оно существует
                    if (club.ClubImage != null)
                    {
                        string oldImagePath = _appEnvironment.ContentRootPath + ev.EventImage.Path;
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        _db.Images.Remove(ev.EventImage);
                    }
                    // сохраняем файл в папку Files в каталоге wwwroot
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    Image image = new Image { ImageName = file.FileName, Path = filePath };
                    ev.EventImage = image;
                    _db.Images.Add(image);
                    _db.SaveChanges();


                    return Ok("Изображение успешно добавлено");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Ошибка при загрузке файла", error = ex.Message });
                }

            }
            var eventDTO = ev.ToDTO();
            return Ok(eventDTO);
        }
        [Authorize]
        [HttpDelete("DeleteEvent")]
        public IActionResult DeleteEvent(string id)
        {
            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == id);
            if (ev == null)
            {
                return BadRequest("Event is not Found!");
            }
            else
            {
                // Ïóòü ê ïàïêå êëóáà
                string eventDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "Files\\Events", ev.Name);

                // Ïðîâåðêà, ñóùåñòâóåò ëè ïàïêà
                if (Directory.Exists(eventDirectoryPath))
                {
                    // Óäàëåíèå ïàïêè è åå ñîäåðæèìîãî
                    Directory.Delete(eventDirectoryPath, true);
                }

                _db.Events.Remove(ev);

                //var EventMemberToRemove = _db.EventMembers.Where(em => em.EventId.ToString() == id);
                //var ClubEventToRemove = _db.ClubEvents.Where(em => em.EventId.ToString() == id);

                //_db.EventMembers.RemoveRange(EventMemberToRemove);
                //_db.ClubEvents.RemoveRange(ClubEventToRemove);
                _db.SaveChanges();
                return Ok();
            }
        }
        [AllowAnonymous]
        [HttpGet("getEvent")]
        public IActionResult getEvent(string id)
        {
            Event? ev = _db.Events.Include(e => e.Members).Include(e => e.EventImage).FirstOrDefault(e => e.Id.ToString() == id);
            if (ev == null)
            {
                return BadRequest("Òàêîãî Èâåíòà íåò :(");
            }
            else
            {
                var eventDTO = ev.ToDTO();
                return Ok(eventDTO);
            }
        }
        [AllowAnonymous]
        [HttpGet("getAllEvents")]
        public IActionResult getAllEvents(string id)
        {
            var events = _db.Events.Include(e => e.Members).Include(e=> e.EventImage).ToList().Where(e => e.ClubID.ToString() == id);
            if (events.Any())
            {
                List<EventDTO> eventDTOs = new List<EventDTO>();

                foreach (var ev in events)
                {
                    var eventDTO = ev.ToDTO();
                    eventDTOs.Add(eventDTO);
                }
                return Ok(eventDTOs);

            }
            else
            {
                return BadRequest("Òàêîãî Èâåíòà íåò :(");
            }
        }        
        
        [Authorize]
        [HttpPost("EditEvent")]
        public IActionResult EditUser(string eventId, string name, string description,string fullDescription)
        {
            var token = HttpContext.GetTokenAsync("access_token");
            var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
            User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
            if (user == null)
            {
                return BadRequest("Íåò òàêîãî ïîëüçîâàòåëÿ îò êîòîðîãî èäåò çàïðîñ :(");
            }
            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
            if (ev == null)
            {
                return BadRequest("Íåò òàêîãî Èâåíòà :(");
            }
            if (ev.CreatorEventID == user.Id || user.Role == Enums.Role.admin)
            {
                ev.Name = name;
                ev.Description = description;     
                ev.FullDescription = fullDescription;
                _db.SaveChanges();
                return Ok("Êëóá óñïåøíî èçìåíåí");
            }
            else
            {
                return BadRequest("Íåò ïðàâ äëÿ èçìåíåíèÿ êëóáà :(");
            }

        }
    }
}
