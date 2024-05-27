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
//using System.Data.Entity;


namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        private readonly IJWTAuthManager _authentication;
        private readonly ApplicationContext _db;

        public EventsController(IJWTAuthManager authentication, ApplicationContext context)
        {
            _authentication = authentication;
            _db = context;
        }
        [Authorize]
        [HttpPost("AddEvent")]
        public IActionResult AddEvent(string name, string description, string fullDescription, string idUser, string idClub)
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
        [HttpPost("EditEvent")]
        public IActionResult EditUser(string eventId, string name, string description,string fullDescription)
        {
            var token = HttpContext.GetTokenAsync("access_token");
            var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
            User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
            if (user == null)
            {
                return BadRequest("Нет такого пользователя от которого идет запрос :(");
            }
            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
            if (ev == null)
            {
                return BadRequest("Нет такого Ивента :(");
            }
            if (ev.CreatorEventID == user.Id || user.Role == Enums.Role.admin)
            {
                ev.Name = name;
                ev.Description = description;     
                ev.FullDescription = fullDescription;
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
