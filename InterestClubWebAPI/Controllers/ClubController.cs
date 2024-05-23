using Microsoft.AspNetCore.Mvc;

namespace InterestClubWebAPI.Controllers
{
    public class ClubController : Controller
    {
        [HttpPost("CreateClub")]
        public IActionResult SingUp(string title, string deskription)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.Club.Any(Club => club.Titlte == title))
                {
                    return BadRequest();
                }
                else
                {                    
                    db.Club.Add(new Club { Title = title, Deksription = deskription });
                    db.SaveChanges();
                    return Ok();
                }
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
