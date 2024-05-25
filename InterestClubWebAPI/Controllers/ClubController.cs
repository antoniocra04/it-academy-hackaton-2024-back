using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
//using System.Data.Entity;
using System.Text.Json;
using System.Xml.Linq;
using static InterestClubWebAPI.Models.User;

namespace InterestClubWebAPI.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class ClubController : Controller
    {
        [HttpPost("CreateClub")]
        public IActionResult CreateClub(string title, string description,string fullDescription, string userId)
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

                    Club club = new Club { Title = title, Description = description,FullDescription = fullDescription, CreatorClubID = user.Id};
                    club.CreatorClubID = user.Id;
                    club.Users.Add(user);
                    db.Clubs.Add(club);                    
                    //UserClub userClub = new UserClub { User = user, Club = club };
                    //db.UserClubs.Add(userClub);                    
                    db.SaveChanges();
                    var clubDTO = club.ToDTO();
                    return Ok(clubDTO);
            }

        }

        [HttpDelete("DeleteClub")]
        public IActionResult DeleteClub(string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Club club = db.Clubs.FirstOrDefault(club => club.Title == title);
                if (club != null)
                {
                    //club.Users.Clear();
                    //club.Events.Clear();
                    db.Clubs.Remove(club);

                    //var userClubsToRemove = db.UserClubs.Where(uc => uc.Club.Title == title);
                    //var ClubEventToRemove = db.UserClubs.Where(ce => ce.Club.Title == title);
                    //db.UserClubs.RemoveRange(ClubEventToRemove);
                    //db.UserClubs.RemoveRange(userClubsToRemove);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("Нет клуба с таким названием");
                }
            }
        }
        
        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Club? club = db.Clubs.Include(c => c.Users).Include(c => c.Events).FirstOrDefault(club => club.Id.ToString() == id);
                if (club == null)
                {
                    return BadRequest("Такого Клуба нет :(");
                }
                else
                {
                    var clubDTO = club.ToDTO();
                    return Ok(clubDTO);
                }
            }
        }
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                
                var clubs = db.Clubs.Include(c => c.Users).Include(c => c.Events).ToList();

                if (clubs.Any())
                {
                    List<ClubDTO> clubDTOs = new List<ClubDTO>();

                    foreach (var club in clubs)
                    {
                        var clubDTO = club.ToDTO();
                        clubDTOs.Add(clubDTO);
                    }
                    
                    return Ok(new { Count = clubDTOs.Count, Clubs = clubDTOs });
                }
                else
                {
                    return BadRequest("Такого Клуба нет :(");
                }
            }
        }
    }
}
