using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class ListAdminController : Controller
    {
        private readonly string _recipesFilePath = Path.Combine("wwwroot", "data", "recipes.json");

        // Phương thức đọc từ file JSON và trả về danh sách các công thức
        private List<AddModel.Recipe> GetRecipesFromJson()
        {
            if (System.IO.File.Exists(_recipesFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(_recipesFilePath);
                return JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }
            return new List<AddModel.Recipe>(); // Trả về danh sách rỗng nếu không có dữ liệu
        }

        // Phương thức hiển thị danh sách công thức
        public IActionResult Index()
        {
            // Lấy dữ liệu từ tệp JSON
            var model = new AddModel
            {
                Recipes = GetRecipesFromJson()
            };

            // Trả về view với model chứa danh sách công thức
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var recipes = GetRecipesFromJson(); // Get the list of recipes
                var recipeToDelete = recipes.FirstOrDefault(r => r.Id == id);

                if (recipeToDelete == null)
                {
                    return NotFound("Recipe not found.");
                }

                recipes.Remove(recipeToDelete); // Remove the recipe from the list

                // Save the updated list back to the file
                var updatedData = JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_recipesFilePath, updatedData);

                return RedirectToAction("Index"); // Redirect back to the index page
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }



    }
}
