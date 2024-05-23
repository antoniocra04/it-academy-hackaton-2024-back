using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Xml.Linq;

namespace InterestClubWebAPI.Controllers
{
    public class ClubController : Controller
    {
        [HttpPost("CreateClub")]
        public IActionResult CreateClub(string title, string description)
        {
            using (ApplicationContext db = new ApplicationContext())
            {                
                if (db.Clubs.Any(Club => Club.Title == title))
                {
                    return BadRequest();
                }
                else
                {                    
                    db.Clubs.Add(new Club { Title = title, Description = description });
                    db.SaveChanges();
                    return Ok();
                }
            }
        }

        [HttpPost("DeleteClub")]
        public IActionResult DeleteClub(string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.Clubs.Any(club => club.Title == title))
                {
                    db.Clubs.Remove(db.Clubs.Where(club => club.Title == title).FirstOrDefault());
                    db.SaveChanges();
                    return Ok();
                }
                else
                {                    
                    return BadRequest();
                }
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
