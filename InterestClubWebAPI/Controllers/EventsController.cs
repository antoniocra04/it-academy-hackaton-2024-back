using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;
using System.Data.Entity;


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

                var userId = Guid.Parse(idUser);
                var clubId = Guid.Parse(idClub);

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                var club = _context.Clubs.FirstOrDefault(c => c.Id == clubId);

                if (user == null || club == null)
                {
                    return BadRequest("User or Club not found.");
                }

                var eventDate = DateTime.Now;
                var ev = new Event { Name = name, Description = description, EventDate = eventDate.ToString() };//

                _context.Events.Add(ev);
                _context.SaveChanges(); // Save changes to generate the Event Id

                var evMem = new EventMember { UserId = user.Id, EventId = ev.Id };
                _context.EventMembers.Add(evMem);

                var clubEv = new ClubEvent { EventId = ev.Id, ClubId = club.Id };
                _context.ClubEvents.Add(clubEv);

                _context.SaveChanges();                
                return Ok(ev);
            }
        }
        [HttpPost("DeleteEvent")]
        public IActionResult DeleteEvent(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if ((db.Events.Find(Guid.Parse(id)) == null))
                {
                    return BadRequest("Event is not Found!");
                }
                else
                {
                    db.Events.Remove(db.Events.Find(Guid.Parse(id)));
                    db.SaveChanges();
                    return Ok();
                }
            }


        }
    }
}
