using Microsoft.AspNetCore.Mvc;

namespace InterestClubWebAPI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
