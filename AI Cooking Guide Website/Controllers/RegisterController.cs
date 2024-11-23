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
            // Load existing users from the JSON file
            List<Users>? existingUsers = LoadUserFromFile("Data/users.json") ?? new List<Users>();

            // Check if email already exists
            if (existingUsers.Any(u => u.Email == users.Email))
            {
                ViewBag.Error = "Email đã được đăng ký. Vui lòng sử dụng email khác.";
                return View();
            }

            // Generate a new unique ID for the user
            users.Id = Guid.NewGuid().ToString();

            // Add the new user to the existing list
            existingUsers.Add(users);

            // Serialize the updated list and save it back to the JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(existingUsers, options);

            using (StreamWriter write = new StreamWriter("Data/users.json"))
            {
                write.WriteLine(jsonString);
            }

            return RedirectToAction("Login", "Login");
        }

        // Method to load users from JSON file
        public List<Users>? LoadUserFromFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                string readText = System.IO.File.ReadAllText(filename);
                return JsonSerializer.Deserialize<List<Users>>(readText);
            }
            return new List<Users>();
        }
    }
}
