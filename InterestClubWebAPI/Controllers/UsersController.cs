using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using System;
//using System.Data.Entity;
using System.Text.Json;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace InterestClubWebAPI.Controllers
{
    
    
    
    //[Route("api/[controller]")]
    //[ApiController]
    public class UsersController : Controller
    {
        [HttpPost("singUp")]
        public IActionResult SingUp(string login, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {

                if (db.Users.Any(user => user.Login == login))
                {
                    return BadRequest();
                }
                else
                {
                    User user = new User { Login = login, Password = password };
                    db.Users.Add(user);
                    db.SaveChanges();
                    var userDTO = user.ToDTO();
                    return Ok(userDTO);
                    
                }
            }
        }

        [HttpPost("loginIn")]
        public IActionResult LoginIn(string login, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    //db.Entry(user).Collection(u => u.UserClubs).Load();
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Не верные данные ");
                }
            }
        }
        [HttpGet("GetUsers")]//Добавить проверку на админа
        public IActionResult GetUsers()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                
                var users = db.Users.ToList();
                //foreach (var user in users)
                //{
                //    db.Entry(user).Collection(u => u.Clubs).Load();
                //    db.Entry(user).Collection(u => u.Events).Load();
                //}
                
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
        }
        [HttpGet("GetUserToId")]
        public IActionResult GetUserToId(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                //User? user = db.Users.Include(db.Users,u => u.Clubs).FirstOrDefault();

                //User? user = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                //   .Include(db.Users, u => u.Clubs ).Include(u => u.Events)
                //   .FirstOrDefault(u => u.Id.ToString() == id);
                User? user = db.Users
    .Include(u => u.Clubs)
    .Include(u => u.Events)
    .FirstOrDefault(u => u.Id.ToString() == id);


                if (user != null)
                {
                    var userDTO = user.ToDTO();
                    return Ok(userDTO);

                }
                else
                {
                    return BadRequest("Нет такого пользователя :(");
                }

            }
        }
        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(string login, string password)
        {


            using (ApplicationContext db = new ApplicationContext())
            {
                User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);


                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return Ok("Пользователь Удален");
                }
                else
                {
                    return BadRequest("Нет такого пользователя :(");
                }
            }
        }

        [HttpPost("EditUser")]
        public IActionResult EditUser(string login,string password,string name,string surname,string fatherland)
        {
            

            using (ApplicationContext db = new ApplicationContext())
            {
               
                User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);


                if (user != null)
                {
                    user.Name = name;
                    user.Surname = surname;
                    user.Fatherland = fatherland;
                    db.SaveChanges();
                    var userDTO = user.ToDTO();
                    return Ok(userDTO);
                }
                else
                {
                    return BadRequest("Нет такого пользователя :(");
                }
            }
        }
    }
}
