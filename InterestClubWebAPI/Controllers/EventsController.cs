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
        public IActionResult AddEvent(string name, string description, string idUser, string idClub)
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
            var ev = new Event { Name = name, Description = description, EventDate = eventDate.ToString() };//
            ev.CreatorEventID = user.Id;
            ev.ClubID = club.Id;
            ev.Members.Add(user);
            _db.Events.Add(ev);
            _db.SaveChanges();
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
                // Путь к папке клуба
                string eventDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "Images", ev.Name);

                // Проверка, существует ли папка
                if (Directory.Exists(eventDirectoryPath))
                {
                    // Удаление папки и ее содержимого
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
            Event? ev = _db.Events.Include(e => e.Members).FirstOrDefault(e => e.Id.ToString() == id);
            if (ev == null)
            {
                return BadRequest("Такого Ивента нет :(");
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
            var events = _db.Events.Include(e => e.Members).ToList().Where(e => e.ClubID.ToString() == id);
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
                return BadRequest("Такого Ивента нет :(");
            }
        }
        [Authorize]
        [HttpPost("AddImageInEvent")]
        public async Task<IActionResult> AddImageInEvent(string eventId)
        {
            var uploadedFile = Request.Form.Files.FirstOrDefault();
            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
            if (ev == null)
            {
                return BadRequest("Такого Ивента нет :(");
            }
            if (uploadedFile != null)
            {
                // Проверка, является ли файл изображением
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();
                if (!permittedExtensions.Contains(ext))
                {
                    return BadRequest("Файл не является изображением");
                }

                // Проверка типа содержимого
                var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!permittedContentTypes.Contains(uploadedFile.ContentType))
                {
                    return BadRequest("Файл не является изображением");
                }

                // Удаление старого изображения, если оно существует
                if (ev.EventImage != null)
                {
                    string oldImagePath = _appEnvironment.ContentRootPath + ev.EventImage.Path;
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    _db.Images.Remove(ev.EventImage);
                }
                // путь к папке Files
                string path = $"/Images/{ev.Name}/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.ContentRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Image image = new Image { ImageName = uploadedFile.FileName, Path = path };
                ev.EventImage = image;
                _db.Images.Add(image);
                _db.SaveChanges();
            }

            return Ok("Изображение успешно добавлено");
        }
    }
}
