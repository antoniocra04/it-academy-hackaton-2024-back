using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using static System.Reflection.Metadata.BlobBuilder;
using InterestClubWebAPI.Repository;
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
    }
}
