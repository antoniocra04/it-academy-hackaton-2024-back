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
                    var guid = new Guid(idUser);
                    User? user = db.Users.FirstOrDefault(u => u.Id == guid);
                    List<User> part = new List <User> { user };
                    db.Events.Add(new Event { Name = name, Description = description, EventDate = eventDate, Participants = part });
                    db.SaveChanges();
                    return Ok();
                }
            }
        }
    }
}
