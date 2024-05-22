﻿using Microsoft.AspNetCore.Mvc;

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
       
        
        [HttpPost("AddEvent")]
        public async Task<ActionResult> AddEvent(Event events)
        {
            dbContext.Events.Add(events);
            dbContext.SaveChanges();
            return Ok(events);
        }
        
        public EventsController(ApplicationContext dbContext, IWebHostEnvironment appEnvironment)
        {
            dbContext = dbContext;
            _appEnvironment = appEnvironment;
        }

    }
}
