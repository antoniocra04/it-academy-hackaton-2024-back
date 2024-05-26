using Dapper;
using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;


namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IJWTAuthManager _authentication;
        private readonly ApplicationContext _db;

        public UsersController(IJWTAuthManager authentication, ApplicationContext context)
        {
            _authentication = authentication;
            _db = context;
        }

        private (string login, string password) getUserCreditansFromJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodeToken = handler.ReadJwtToken(token);
            var login = decodeToken.Claims.First(claim => claim.Type == "login").Value;
            var password = decodeToken.Claims.First(claim => claim.Type == "password").Value;
            return (login, password);
        }

        [AllowAnonymous]
        [HttpPost("singUp")]
        public IActionResult SingUp(string login, string password)
        {
            if (_db.Users.Any(user => user.Login == login))
            {
                return BadRequest("Такой пользователь уже существует");
            }
            else
            {                
                
                User user = new User { Login = login, Password = password };
                _db.Users.Add(user);
                _db.SaveChanges();
                var userDTO = user.ToDTO();
                var tokenResponse = _authentication.GenerateJWT(user);

                if (tokenResponse.code == 200)
                {
                    var response = new
                    {
                        access_token = tokenResponse.Data,
                        user = userDTO
                    };
                    return Ok(response);
                }
                return BadRequest(tokenResponse.message);  
            }
        }

        [AllowAnonymous]
        [HttpPost("logIn")]
        public IActionResult LogIn(string login, string password)
        {
            var user = _db.Users
                .Include(u => u.Clubs)
                .Include(u => u.Events)
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                var tokenResponse = _authentication.GenerateJWT(user);
                if (tokenResponse.code == 200)
                {
                    var response = new
                    {
                        access_token = tokenResponse.Data,
                        user = user.ToDTO()
                    };
                
                    return Ok(response);
                }
                return BadRequest(tokenResponse.message);
            }
            return BadRequest("Введен неправильный пароль, либо такого пользователя не существует");
        }
        [Authorize]
        [HttpGet("GetUsers")]//Добавить проверку на админа
        public IActionResult GetUsers()
        {
            var users = _db.Users.ToList();
            if (users.Any())
            {
                List<UserDTO> userDTOs = new List<UserDTO>();

                foreach (var user in users)
                {
                    var userDTO = user.ToDTO();
                    userDTOs.Add(userDTO);
                }
                return Ok(userDTOs);
            }
            else
            {
                return BadRequest("Нет пользователей :(");
            }
        }
        [Authorize]
        [HttpGet("GetUserById")]
        public IActionResult GetUserToId(string id)
        {
            User? user = _db.Users
.Include(u => u.Clubs)
.Include(u => u.Events)
.FirstOrDefault(u => u.Id.ToString() == id);

            if (user != null)
            {
                var userDTO = user.ToDTO();
                return Ok(userDTO.Role.ToString());

            }
            else
            {
                return BadRequest("Нет такого пользователя :(");
            }
        }

        [Authorize]
        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(string login, string password)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            //HttpContext.Response.Headers
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok("Пользователь Удален");
            }
            else
            {
                return BadRequest("Нет такого пользователя :(");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteUserByAdmin")]
        public IActionResult DeleteUser(string adminLogin, string adminPassword, string userLogin)
        {
            User? adminUser = _db.Users.FirstOrDefault(u => u.Login == adminLogin && u.Password == adminPassword);
            if (adminUser == null)
            {
                return BadRequest($"Нет админа под логином {adminLogin}");
            }
            if (adminUser.Role != Enums.Role.admin)
            {
                return BadRequest("Нет прав админа");
            }
            User? user = _db.Users.FirstOrDefault(u => u.Login == userLogin);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok("Пользователь Удален");
            }
            else
            {
                return BadRequest($"Нет пользователя под логином: {userLogin}"); ; ;
            }

        }
        [Authorize]
        [HttpPost("EditUser")]
        public IActionResult EditUser(string login, string password, string name, string surname, string fatherland)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                user.Name = name;
                user.Surname = surname;
                user.Fatherland = fatherland;
                _db.SaveChanges();
                var userDTO = user.ToDTO();
                return Ok(userDTO);
            }
            else
            {
                return BadRequest("Нет такого пользователя :(");
            }
        }

        [Authorize]
        [HttpPost("JoinInClub")]
        public IActionResult JoinInClub(string clubId)
        {
            var token = HttpContext.GetTokenAsync("access_token");
            var userCreditans = getUserCreditansFromJWT(token.Result);
            User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
            if (user == null)
            {
                return BadRequest($"Нет пользователя с логином: {userCreditans.login}.Или неверный пароль");
            }
            Club? club = _db.Clubs.FirstOrDefault(c => c.Id.ToString() == clubId);
            if (club == null)
            {
                return BadRequest("Нет клуба с таким ID");
            }
            club.Users.Add(user);
            _db.SaveChanges();
            return Ok(user);
        }

        [Authorize]
        [HttpPost("ExitFromClub")]
        public IActionResult ExitFromClub(string login, string password, string clubId)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return BadRequest($"Нет пользователя с логином: {login}.Или неверный пароль");
            }
            Club? club = _db.Clubs.FirstOrDefault(c => c.Id.ToString() == clubId);
            if (club == null)
            {
                return BadRequest("Нет клуба с таким ID");
            }
            club.Users.Remove(user);
            _db.SaveChanges();
            return Ok(user);
        }

        [Authorize]
        [HttpPost("JoinInEvent")]
        public IActionResult JoinInEvent(string login, string password, string eventId)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return BadRequest($"Нет пользователя с логином: {login}.Или неверный пароль");
            }

            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
            if (ev == null)
            {
                return BadRequest("Нет Ивента с таким ID");
            }
            ev.Members.Add(user);
            _db.SaveChanges();
            return Ok(user);
        }
        [Authorize]
        [HttpPost("ExitFromEvent")]
        public IActionResult ExitFromEvent(string login, string password, string eventId)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return BadRequest($"Нет пользователя с логином: {login}.Или неверный пароль");
            }
            Event? ev = _db.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
            if (ev == null)
            {
                return BadRequest("Нет Ивента с таким ID");
            }
            ev.Members.Remove(user);
            _db.SaveChanges();
            return Ok(user);

        }
    }
}

