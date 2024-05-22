using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Xml.Linq;

namespace InterestClubWebAPI.Controllers
{
    public class UsersController : Controller    {
            
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("singUp")]
        public IActionResult SingUp(string login, string password)
        {
            using (ApplicationContext db = new ApplicationContext())
            {               
                
                if (db.Users.Any(p => p.Login == login))
                {
                    return BadRequest();
                }
                else
                {
                    //стоит дороботать логику сопостовления паролей и не занятость логина 
                    db.Users.Add(new User { Login = login, Password = password });
                    db.SaveChanges();
                    return Ok();
                }   
            }  
                
        }

    }
}
