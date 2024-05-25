using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IJWTAuthManager _authentication;
        public ClubController(IJWTAuthManager authentication, ApplicationContext context)
        {
            _authentication = authentication;
            _db = context;
        }

        [HttpPost("CreateClub")]
        public IActionResult CreateClub(string title, string description, string fullDescription, string userId)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (_db.Clubs.Any(c => c.Title == title))
            {
                return BadRequest("Club with the same title already exists.");
            }
            Club club = new Club { Title = title, Description = description, FullDescription = fullDescription, CreatorClubID = user.Id };
            club.CreatorClubID = user.Id;
            club.Users.Add(user);
            _db.Clubs.Add(club);
            _db.SaveChanges();
            var clubDTO = club.ToDTO();
            return Ok(clubDTO);
        }

        [HttpDelete("DeleteClub")]
        public IActionResult DeleteClub(string title)
        {

            Club? club = _db.Clubs.FirstOrDefault(club => club.Title == title);
            if (club != null)
            {
                _db.Clubs.Remove(club);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Нет клуба с таким названием");
            }
        }

        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {

            Club? club = _db.Clubs.Include(c => c.Users).Include(c => c.Events).FirstOrDefault(club => club.Id.ToString() == id);
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
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            var clubs = _db.Clubs.Include(c => c.Users).Include(c => c.Events).ToList();

            if (clubs.Any())
            {
                List<ClubDTO> clubDTOs = new List<ClubDTO>();

                foreach (var club in clubs)
                {
                    var clubDTO = club.ToDTO();
                    clubDTOs.Add(clubDTO);
                }

                return Ok(clubDTOs);
            }
            else
            {
                return BadRequest("Такого Клуба нет :(");
            }
        }
    }
}
