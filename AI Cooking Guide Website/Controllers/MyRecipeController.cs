using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class MyRecipeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] MyRecipeModel recipe, IFormFile image)
        {
            // Kiểm tra trạng thái đăng nhập qua Identity
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Bạn cần phải đăng nhập để lưu công thức.";
                return RedirectToAction("Login", "Login");
            }

            try
            {
                // Lấy tên người dùng từ Identity
                var userName = User.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                {
                    TempData["ErrorMessage"] = "Tên người dùng không xác định. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Login");
                }

                recipe.UserName = userName;

                // Check if image is provided and has valid extension
                if (image == null || image.Length == 0)
                {
                    return BadRequest("Hãy chọn ảnh để tải lên.");
                }

                var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();

                if (!validExtensions.Contains(fileExtension))
                {
                    return BadRequest("Chỉ hỗ trợ ảnh với định dạng .jpg, .jpeg, .png.");
                }

                var imagesDirectory = Path.Combine("wwwroot", "mrimages");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory); // Create directory if it doesn't exist
                }

                var imagePath = Path.Combine(imagesDirectory, image.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                recipe.Image = "/mrimages/" + image.FileName;
                recipe.UserName = userName;  // Lưu tên người dùng vào công thức

                // Directly use Ingredients and Steps as lists
                if (recipe.Ingredients != null)
                {
                    recipe.Ingredients = recipe.Ingredients.Select(i => i.Trim()).ToList();
                }
                if (recipe.Steps != null)
                {
                    recipe.Steps = recipe.Steps.Select(s => s.Trim()).ToList();
                }

                // Read existing recipes from the JSON file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "myrecipe.json");
                var recipes = LoadMyRecipesFromFile(filePath) ?? new List<MyRecipeModel>();

                // Assign a new Id based on the highest existing Id in the list
                recipe.Id = recipes.Any() ? recipes.Max(r => r.Id) + 1 : 1;

                recipes.Add(recipe);

                // Save the updated recipes list to JSON
                var updatedData = JsonSerializer.Serialize(recipes);
                System.IO.File.WriteAllText(filePath, updatedData);

                TempData["SuccessMessage"] = "Công thức của bạn đã được thêm thành công!"; // Add success message to TempData
                return RedirectToAction("Profile", "Profile"); // Redirect to Profile page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu công thức: " + ex.Message; // Store error message in TempData
                return RedirectToAction("Index"); // Redirect back to the form if an error occurs
            }
        }

        // Helper method to load recipes from JSON file
        private List<MyRecipeModel>? LoadMyRecipesFromFile(string fileName)
        {
            try
            {
                string readText = System.IO.File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<List<MyRecipeModel>>(readText);
            }
            catch (Exception)
            {
                return null; // Return null if there is an error reading the file
            }
        }
    }
}
