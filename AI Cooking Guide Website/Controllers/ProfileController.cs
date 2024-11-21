using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class ProfileController : Controller
    {
        // Profile action: Loads user data and user-specific recipes
        public IActionResult Profile()
        {
            // Get the username of the currently logged-in user
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Login"); // Redirect to login if user is not authenticated
            }

            // Load the users from JSON and find the logged-in user
            var user = LoadUsersFromFile("Data/users.json")?.FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                // Load all recipes from myrecipe.json
                var recipes = LoadMyRecipesFromFile("Data/myrecipe.json");

                // Filter the recipes for the logged-in user
                var userRecipes = recipes?.Where(r => r.UserName == userName).ToList();

                // Create a ViewModel to pass to the view
                var profileViewModel = new ProfileViewModel
                {
                    User = user,
                    Recipes = userRecipes ?? new List<MyRecipeModel>()
                };

                // Add success or error message to the view
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

                // Pass ViewModel to the view
                return View(profileViewModel);
            }

            // If user is not found, redirect to login page
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult UpdateProfile(string PhoneNumber, string Address)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                TempData["ErrorMessage"] = "You need to be logged in to update your profile.";
                return RedirectToAction("Login", "Login"); // Redirect to login if user is not authenticated
            }

            // Load users from file
            var userList = LoadUsersFromFile("Data/users.json");

            if (userList == null)
            {
                TempData["ErrorMessage"] = "Unable to load user data.";
                return RedirectToAction("Profile"); // Go back to profile page if loading fails
            }

            // Find the user and update details
            var user = userList?.FirstOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                // Update the user's phone number and address
                user.PhoneNumber = PhoneNumber;
                user.Address = Address;

                // Save updated list back to JSON file
                SaveUsersToFile("Data/users.json", userList);

                TempData["SuccessMessage"] = "Thông tin của bạn đã được cập nhật thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
            }

            // Redirect to Profile page after saving changes
            return RedirectToAction("Profile");
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


    public IActionResult RecipeDetails(int id)
        {
            // Load recipes from JSON file
            var recipes = LoadMyRecipesFromFile("Data/myrecipe.json");

            if (recipes == null || !recipes.Any())
            {
                return NotFound("No recipes found."); // Return 404 if file is empty or not found
            }

            // Find the recipe by ID
            var recipe = recipes.FirstOrDefault(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound($"Recipe with ID {id} not found."); // Return 404 if recipe is not found
            }

            // Pass the recipe to the view
            return View(recipe);
        }

        // Helper method to load recipes from JSON file
        private List<MyRecipeModel>? LoadMyRecipesFromFile(string fileName)
        {
            try
            {
                string readText = System.IO.File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<List<MyRecipeModel>>(readText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {fileName}: {ex.Message}");
                return null;
            }
        }

    }
}
