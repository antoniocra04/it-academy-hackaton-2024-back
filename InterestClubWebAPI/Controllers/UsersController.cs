using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity;
using System.Xml.Linq;

namespace InterestClubWebAPI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

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
                    return Ok(user);
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
                    return Ok(user);
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
