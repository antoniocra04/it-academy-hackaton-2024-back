using InterestClubWebAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Xml.Linq;

namespace InterestClubWebAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationContext dbContext;
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("singUp")]
        public IActionResult SingUp(string login, string password, string confirmation_pasword)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmation_pasword))// защита от пустых строк
                return BadRequest();
           //var user = dbContext.Users.Where(p => p.Login == login).ToList();
            if (!dbContext.Users.Any(p => p.Login == login ) && confirmation_pasword == password )// проверка свободного логина и соответствия поролей
            {
                dbContext.Users.Add(new Models.User { Login=login, Password= password});
                dbContext.SaveChanges();
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
