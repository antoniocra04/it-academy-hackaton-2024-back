using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;


namespace InterestClubWebAPI.Controllers
{
    public class EventsController : Controller
    {    
        [HttpPost("AddEvent")]
        public IActionResult AddEvent(string name, string description, string idUser, string idClub)
        {
            using (ApplicationContext db = new ApplicationContext())
            {                               
                if (db.Events.Any(Event => Event.Name == name))
                {
                    return BadRequest();
                }
                else
                {    
                    DateTime eventDate = DateTime.Now;
                    var IdU = new Guid(idUser);
                    var IdC = new Guid(idClub);
                    User? user = db.Users.FirstOrDefault(u => u.Id == IdU);
                    Club? club = db.Clubs.FirstOrDefault(c => c.Id == IdC);
                    Event ev = new Event { Name = name, Description = description, EventDate = eventDate};
                    db.Events.Add(ev);
                    db.SaveChanges();
                    Event? even = db.Users.FirstOrDefault(e => e.Name == name);
                    EventMember evMem = new EventMember { UserId = IdU, User = user, EventId = even.Id, Event = even};
                    db.EventMembers.Add(evMem);
                    db.SaveChanges();
                    ClubEvent clubEv = new ClubEvent { EventId = even.Id, Event = even, ClubId = IdC, Club = club };
                    db.ClubEvents.Add(clubEv);
                    db.SaveChanges();
                    return Ok();
                }
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
