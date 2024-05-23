using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Xml.Linq;
using static InterestClubWebAPI.Models.User;

namespace InterestClubWebAPI.Controllers
{
    public class ClubController : Controller
    {
        [HttpPost("CreateClub")]
        public IActionResult CreateClub(string title, string description, string userId)
        {
            
                using (ApplicationContext db = new ApplicationContext())
                {
                    User? user = db.Users.FirstOrDefault(u => u.Id.ToString() == userId);

                    if (user == null)
                    {
                        return BadRequest("User not found.");
                    }

                    if (db.Clubs.Any(c => c.Title == title))
                    {
                        return BadRequest("Club with the same title already exists.");
                    }

                    Club club = new Club { Title = title, Description = description };
                    db.Clubs.Add(club);

                    UserClub userClub = new UserClub { User = user, Club = club };
                    db.UserClubs.Add(userClub);

                    db.SaveChanges();
                    return Ok(club);
                }
            
        }

        [HttpPost("DeleteClub")]
        public IActionResult DeleteClub(string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Club club = db.Clubs.FirstOrDefault(club => club.Title == title);
                if (club != null)
                {
                    db.Clubs.Remove(club);

                    var userClubsToRemove = db.UserClubs.Where(uc => uc.Club.Title == title);
                    db.UserClubs.RemoveRange(userClubsToRemove);
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
