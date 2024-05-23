using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;
using Microsoft.Extensions.Logging;

namespace InterestClubWebAPI.Controllers
{
    public class EventsController : Controller
    {
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
