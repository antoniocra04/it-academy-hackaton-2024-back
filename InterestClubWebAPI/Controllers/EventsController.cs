using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Context;

namespace InterestClubWebAPI.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationContext dbContext;
        IWebHostEnvironment _appEnvironment;
        public IActionResult Index()
        {
            return View();
        }
       

        [HttpPost("DelEvent")]
        public async Task<ActionResult> DelEvent(Event events) 
        {

            if ((dbContext.Events.Find(events.Id) == null))
            {
                return BadRequest("Event is not Found!");
            }
            else
            {
                dbContext.Events.Remove(events);
                dbContext.SaveChanges();
                return Ok();
            }
           

        }
        
        public EventsController(ApplicationContext dbContext, IWebHostEnvironment appEnvironment)
        {
            dbContext = dbContext;
            _appEnvironment = appEnvironment;
        }



    }
}
