using Microsoft.AspNetCore.Mvc;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
