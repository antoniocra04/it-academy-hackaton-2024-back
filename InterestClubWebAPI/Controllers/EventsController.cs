using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using static System.Reflection.Metadata.BlobBuilder;
//using System.Data.Entity;


namespace InterestClubWebAPI.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class EventsController : Controller
    {
        [HttpPost("AddEvent")]
        public IActionResult AddEvent(string name, string description, string idUser, string idClub)
        {
            using (ApplicationContext _context = new ApplicationContext())
            {
                if (_context.Events.Any(e => e.Name == name))
                {
                    return BadRequest("Event with the same name already exists.");
                }
                
                User? user = _context.Users.FirstOrDefault(u => u.Id.ToString() == idUser);
                Club? club = _context.Clubs.FirstOrDefault(c => c.Id.ToString() == idClub);

                if (user == null || club == null)
                {
                    return BadRequest("User or Club not found.");
                }

                var eventDate = DateTime.Now;
                var ev = new Event { Name = name, Description = description, EventDate = eventDate.ToString() };//
                ev.CreatorEventID = user.Id;
                ev.ClubID = club.Id;
                ev.Members.Add(user);
                _context.Events.Add(ev);
                _context.SaveChanges();
                var eventDTO = ev.ToDTO();
                return Ok(eventDTO);
            }
        }
        [HttpDelete("DeleteEvent")]
        public IActionResult DeleteEvent(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Event? ev = db.Events.FirstOrDefault(e => e.Id.ToString() == id);
                if (ev == null)
                {
                    return BadRequest("Event is not Found!");
                }
                else
                {                    
                    db.Events.Remove(ev);

                    //var EventMemberToRemove = db.EventMembers.Where(em => em.EventId.ToString() == id);
                    //var ClubEventToRemove = db.ClubEvents.Where(em => em.EventId.ToString() == id);

                    //db.EventMembers.RemoveRange(EventMemberToRemove);
                    //db.ClubEvents.RemoveRange(ClubEventToRemove);
                    db.SaveChanges();
                    return Ok();
                }
            }


        }

        [HttpGet("getEvent")]
        public IActionResult getEvent(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Event? ev = db.Events.Include(e => e.Members).FirstOrDefault(e => e.Id.ToString() == id);
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
        }
        [HttpGet("getAllEvents")]
        public IActionResult getAllEvents(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var events = db.Events.Include(e => e.Members).ToList().Where(e => e.ClubID.ToString() == id);                
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
}
