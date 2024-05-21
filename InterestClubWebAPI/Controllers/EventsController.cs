using Microsoft.AspNetCore.Mvc;

namespace InterestClubWebAPI.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
