using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class ListAdminController : Controller
    {
        private readonly string _recipesFilePath = Path.Combine("wwwroot", "data", "recipes.json");

        // Method to read the recipes from JSON file
        private List<AddModel.Recipe> GetRecipesFromJson()
        {
            if (System.IO.File.Exists(_recipesFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(_recipesFilePath);
                return JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }
            return new List<AddModel.Recipe>(); // Return an empty list if no data exists
        }

        // Method to display the list of recipes
        public IActionResult Index()
        {
            var model = new AddModel
            {
                Recipes = GetRecipesFromJson()
            };
            return View(model);
        }

        // Method to delete a recipe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var recipes = GetRecipesFromJson();
                var recipeToDelete = recipes.FirstOrDefault(r => r.Id == id);

                if (recipeToDelete == null)
                {
                    return NotFound("Recipe not found.");
                }

                recipes.Remove(recipeToDelete); // Remove the recipe from the list

                // Save the updated list back to the file
                var updatedData = JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_recipesFilePath, updatedData);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // Method to show the edit form (GET request)
        [HttpGet("ListAdmin/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var recipes = GetRecipesFromJson();
            var recipe = recipes.FirstOrDefault(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound("Recipe not found.");
            }

            return View(recipe); // Return the edit view with the recipe data
        }

        [HttpPost("ListAdmin/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AddModel.Recipe updatedRecipe, IFormFile image)
        {
            try
            {
                var recipes = GetRecipesFromJson();
                var recipeToEdit = recipes.FirstOrDefault(r => r.Id == id);

                if (recipeToEdit == null)
                {
                    return NotFound("Recipe not found.");
                }

                // Convert Ingredients and Steps to List<string> by splitting based on commas
                updatedRecipe.Ingredients = !string.IsNullOrWhiteSpace(Request.Form["Ingredients"])
                    ? Request.Form["Ingredients"].ToString().Split(',').Select(i => i.Trim()).ToList()
                    : new List<string>();

                updatedRecipe.Steps = !string.IsNullOrWhiteSpace(Request.Form["Steps"])
                    ? Request.Form["Steps"].ToString().Split(',').Select(s => s.Trim()).ToList()
                    : new List<string>();

                // If a new image is uploaded, save it
                if (image != null && image.Length > 0)
                {
                    var imagesDirectory = Path.Combine("wwwroot", "images");
                    if (!Directory.Exists(imagesDirectory))
                    {
                        Directory.CreateDirectory(imagesDirectory); // Create directory if it doesn't exist
                    }

                    var imagePath = Path.Combine(imagesDirectory, image.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    // Update image path
                    updatedRecipe.Image = "/images/" + image.FileName;
                }

                // Update the recipe details
                recipeToEdit.Name = updatedRecipe.Name;
                recipeToEdit.Ingredients = updatedRecipe.Ingredients;
                recipeToEdit.Description = updatedRecipe.Description;
                recipeToEdit.Steps = updatedRecipe.Steps;

                if (!string.IsNullOrEmpty(updatedRecipe.Image))
                {
                    recipeToEdit.Image = updatedRecipe.Image;
                }

                // Save the updated list back to the file
                var updatedData = JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_recipesFilePath, updatedData);

                return RedirectToAction("Index"); // Redirect back to the list of recipes
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}
