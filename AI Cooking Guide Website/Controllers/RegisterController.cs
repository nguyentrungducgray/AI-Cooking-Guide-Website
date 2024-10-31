using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class RegisterController : Controller
    {
        static List<Users> register = new List<Users>();
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Users users)
        {
            register.Add(users);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(register, options);

            //save file 
            using (StreamWriter write = new StreamWriter("Data/users.json"))
            {
                write.WriteLine(jsonString);
            }
            return RedirectToAction("Login", "Login"); ;
        }
        public List<Users>? LoadUserFromFile(string filename)
        {
            string readText = System.IO.File.ReadAllText("Data/users.json");
            return JsonSerializer.Deserialize<List<Users>>(readText);
        }
    }
}
