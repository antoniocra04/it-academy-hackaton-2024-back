using Microsoft.AspNetCore.Mvc;

namespace InterestClubWebAPI.Controllers
{
    public class ClubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
