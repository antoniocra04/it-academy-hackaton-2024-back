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
            try
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
                        string rezult;
                        rezult = await SaveFileModel.SaveFile(_appEnvironment.ContentRootPath, "Events", ev.Name, file);
                        if (rezult != "Изображение успешно сохранено")
                        {
                            return BadRequest(rezult);
                        }

                        string imageUrl = Url.Content("~/StaticFiles/Events/" + ev.Name + "/" + file.FileName);
                        Image image = new Image { ImageName = file.FileName, Path = imageUrl };
                        ev.EventImage = image;
                        _db.Images.Add(image);
                        _db.SaveChanges();


                        //return Ok(new { message = "Изображение успешно добавлено", imageUrl });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = "Ошибка при загрузке файла", error = ex.Message });
                    }

                }
                var eventDTO = ev.ToDTO();
                return Ok(eventDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("DeleteEvent")]
        public IActionResult DeleteEvent(string id)
        {
            try
            {
                Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == id);
                if (ev == null)
                {
                    return BadRequest("Event is not Found!");
                }
                else
                {
                    // Ïóòü ê ïàïêå êëóáà
                    string eventDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "MyStaticFiles\\Events", ev.Name);

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
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("getEvent")]
        public IActionResult getEvent(string id)
        {
            try
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
            catch (Exception ex)
{
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("getAllEvents")]
        public IActionResult getAllEvents(string id)
        {
            try
            {
                var events = _db.Events.Include(e => e.Members).Include(e => e.EventImage).ToList().Where(e => e.ClubID.ToString() == id);
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
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }        
        
        [Authorize]
        [HttpPost("EditEvent")]
        public async Task<IActionResult> EditUser(string eventId, string name, string description,string fullDescription,IFormFile? file)
        {
            try
            {
                var token = HttpContext.GetTokenAsync("access_token");
                var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
                User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
                if (user == null)
                {
                    return BadRequest("Íåò òàêîãî ïîëüçîâàòåëÿ îò êîòîðîãî èäåò çàïðîñ :(");
                }
                Event? ev = _db.Events.Include(e => e.EventImage).FirstOrDefault(e => e.Id.ToString() == eventId);
                if (ev == null)
                {
                    return BadRequest("Íåò òàêîãî Èâåíòà :(");
                }
                if (ev.CreatorEventID == user.Id || user.Role == Enums.Role.admin)
                {
                    if (file != null)
                    {
                        // Удаление старого изображения, если оно существует
                        if (ev.EventImage != null)
                        {
                            // Путь к папке клуба
                            string oldImagePath = Path.Combine(_appEnvironment.ContentRootPath, "MyStaticFiles\\Events", ev.Name);

                            // Проверка, существует ли папка
                            if (Directory.Exists(oldImagePath))
                            {
                                // Удаление папки и ее содержимого
                                Directory.Delete(oldImagePath, true);
                            }
                            _db.Images.Remove(ev.EventImage);
                        }
                        ev.Name = name;
                        string rezult;
                        rezult = await SaveFileModel.SaveFile(_appEnvironment.ContentRootPath, "Events", ev.Name, file);
                        if (rezult != "Изображение успешно сохранено")
                        {
                            return BadRequest(rezult);
                        }
                    }
                    string imageUrl = Url.Content("~/StaticFiles/Events/" + ev.Name + "/" + file.FileName);
                    Image image = new Image { ImageName = file.FileName, Path = imageUrl };
                    ev.Name = name;
                    ev.Description = description;
                    ev.FullDescription = fullDescription;
                    ev.EventImage = image;
                    _db.Images.Add(image);
                    _db.SaveChanges();
                    return Ok("Êëóá óñïåøíî èçìåíåí");
                }
                else
                {
                    return BadRequest("Íåò ïðàâ äëÿ èçìåíåíèÿ êëóáà :(");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }

        }
    }
}
