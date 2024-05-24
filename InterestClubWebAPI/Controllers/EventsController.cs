﻿using Microsoft.AspNetCore.Mvc;

namespace InterestClubWebAPI.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }       
        
        [HttpPost("AddEvent")]
        public IActionResult AddEvent(string name, string description, string idUser)
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
                    User? user = db.Users.FirstOrDefault(u => u.Id == IdU);
                    List<User> part = new List <User> { user };
                    Event ev = new Event { Name = name, Description = description, EventDate = eventDate};
                    db.Events.Add(ev);
                    db.SaveChanges();
                    Event? even = db.Users.FirstOrDefault(e => e.Name == name);
                    EventMember evMem = new EventMember { UserId = IdU, User = user, EventId = even.Id, Event = even};
                    db.Events.Add(ev);
                    db.SaveChanges();
                    return Ok();
                }
            }
        }
    }
}
