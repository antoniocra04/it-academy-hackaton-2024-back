using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.DTOs;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
        [Authorize]
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
        [Authorize]
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

        [AllowAnonymous]
        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {

            Club? club = _db.Clubs.Include(c => c.Users).Include(c => c.Events).Include(c => c.Discussions).FirstOrDefault(club => club.Id.ToString() == id);
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
        [AllowAnonymous]
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            var clubs = _db.Clubs.Include(c => c.Users).Include(c => c.Events).Include(c => c.Discussions).ToList();

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
        [Authorize]
        [HttpPost("EditClub")]
        public IActionResult EditUser(string clubId,string title, string description, string fullDescription)
        {
            var token = HttpContext.GetTokenAsync("access_token");
            var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
            User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
            if(user == null)
            {
                return BadRequest("Нет такого пользователя от которого идет запрос :(");
            }
            Club? club = _db.Clubs.FirstOrDefault(c => c.Id.ToString()== clubId);
            if(club == null)
            {
                return BadRequest("Нет такого клуба :(");
            }
            if(club.CreatorClubID == user.Id || user.Role == Enums.Role.admin) 
            {
                club.Title = title;
                club.Description = description;
                club.FullDescription = fullDescription;
                _db.SaveChanges();                
                return Ok("Клуб успешно изменен");
            }
            else
            {
                return BadRequest("Нет прав для изменения клуба :(");
            }            
        }
    }
}
