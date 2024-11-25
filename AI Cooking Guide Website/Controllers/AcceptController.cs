using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Hosting;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class AcceptController : Controller
    {
        private readonly IHostEnvironment _hostEnvironment;

        // Constructor để inject IHostEnvironment
        public AcceptController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        // Action để hiển thị các món ăn từ file JSON
        public IActionResult Index()
        {
            var myRecipeFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "myrecipe.json");
            var recipesFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "data", "recipes.json");

            if (!System.IO.File.Exists(myRecipeFilePath))
            {
                return NotFound("File myrecipe.json không tồn tại");
            }

            var jsonData = System.IO.File.ReadAllText(myRecipeFilePath);
            var myRecipes = JsonConvert.DeserializeObject<List<MyRecipeModel>>(jsonData);

            return View(myRecipes);
        }

        // Action để xử lý Accept và thêm món ăn vào recipes.json
        public IActionResult AcceptRecipe(int id)
        {
            // Đọc dữ liệu từ các file JSON
            var myRecipeFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "Data", "myrecipe.json");
            var recipesFilePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "data", "recipes.json");

            // Đọc nội dung file myrecipe.json
            var myRecipesJson = System.IO.File.ReadAllText(myRecipeFilePath);
            var myRecipes = JsonConvert.DeserializeObject<List<MyRecipeModel>>(myRecipesJson);

            // Tìm món ăn trong myrecipe.json theo ID
            var recipeToAccept = myRecipes.FirstOrDefault(r => r.Id == id);
            if (recipeToAccept != null)
            {
                // Đánh dấu món ăn là đã duyệt
                recipeToAccept.IsAccepted = true;

                // Lưu lại trạng thái đã duyệt vào myrecipe.json
                var updatedMyRecipesJson = JsonConvert.SerializeObject(myRecipes, Formatting.Indented);
                System.IO.File.WriteAllText(myRecipeFilePath, updatedMyRecipesJson);

                // Cập nhật file recipes.json nếu cần (giống như trước)
                var recipesJson = System.IO.File.ReadAllText(recipesFilePath);
                var recipes = JsonConvert.DeserializeObject<List<MyRecipeModel>>(recipesJson);

                // Lấy ID tự động từ món ăn cuối cùng trong recipes.json
                var nextId = recipes.Any() ? recipes.Max(r => r.Id) + 1 : 1;

                // Gán ID tự động cho món ăn
                recipeToAccept.Id = nextId;

                // Thêm món ăn vào danh sách recipes
                recipes.Add(recipeToAccept);

                // Lưu lại danh sách mới vào recipes.json
                var updatedRecipesJson = JsonConvert.SerializeObject(recipes, Formatting.Indented);
                System.IO.File.WriteAllText(recipesFilePath, updatedRecipesJson);
            }

            // Quay lại trang Index sau khi duyệt
            return RedirectToAction("Index");
        }


    }
}
