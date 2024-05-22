using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Xml.Linq;

namespace InterestClubWebAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationContext dbContext;
        public UsersController(ApplicationContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("singUp")]
        public IActionResult SingUp(string login, string password, string confirmation_pasword)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmation_pasword))// защита от пустых строк
                return NotFound();
            //var user = dbContext.Users.Where(p => p.Login == login).ToList();

            #region Тест 
            // задел на допил после создания frontend части
            if (dbContext.Users.Any(p => p.Login == login))
            {
                //вернуть сообщение что логин занят
            }
            if(confirmation_pasword == password)
            {
                // вернуть сообщение о не соответствии паролей
            }
            #endregion

            if (!dbContext.Users.Any(p => p.Login == login ) && confirmation_pasword == password )// проверка свободного логина и соответствия поролей
            {
                //стоит дороботать логику сопостовления паролей и не занятость логина 
                dbContext.Users.Add(new User { Login=login, Password= password});
                dbContext.SaveChanges();
                return Ok();
            }
            else
                return BadRequest();
        }

    }
}
