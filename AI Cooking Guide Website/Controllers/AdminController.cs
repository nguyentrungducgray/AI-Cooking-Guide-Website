using Microsoft.AspNetCore.Mvc;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
