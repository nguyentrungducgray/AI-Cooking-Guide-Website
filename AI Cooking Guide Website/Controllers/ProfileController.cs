using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            // Fetch username from session or claims (assuming it's set in the login process)
            var userName = User.Identity.Name;
            var user = LoadUsersFromFile("Data/users.json")?.FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                return View(user); // Pass user model to the Profile view
            }
            return RedirectToAction("Login", "Login"); // Redirect if user not fou
        }
        [HttpPost]
        public IActionResult UpdateProfile(string PhoneNumber, string Address)
        {
            var userName = User.Identity.Name;
            var userList = LoadUsersFromFile("Data/users.json");

            // Find the user and update details
            var user = userList?.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                user.PhoneNumber = PhoneNumber;
                user.Address = Address;

                // Save updated list back to JSON file
                SaveUsersToFile("Data/users.json", userList);

                // Set success message
                TempData["SuccessMessage"] = "Thông tin của bạn đã được cập nhật thành công!";
            }

            // Redirect to Profile page after saving changes
            return RedirectToAction("Index");
        }

        // Load users from JSON
        private List<Users>? LoadUsersFromFile(string fileName)
        {
            string readText = System.IO.File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<Users>>(readText);
        }

        // Save updated users list to JSON
        private void SaveUsersToFile(string fileName, List<Users> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(fileName, json);
        }


	}
}
