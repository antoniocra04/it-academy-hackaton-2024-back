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
        public IActionResult CreateClub(string title, string deskription)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.Club.Any(Club => club.Titlte == title))
                {
                    return BadRequest();
                }
                else
                {                    
                    db.Club.Add(new Club { Title = title, Deksription = deskription });
                    db.SaveChanges();
                    return Ok();
                }
            }
        }

        [HttpPost("delClub")]
        public IActionResult DelClub(string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.Club.Any(club => club.Title == title))
                {
                    db.Club.Remove(db.Club.Where(club => club.Title == title).FirstOrDefault());
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
