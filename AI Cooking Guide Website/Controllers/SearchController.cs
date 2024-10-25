using AI_Cooking_Guide_Website.ModelAI;
using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class Search : Controller
    {
        private readonly RecipeApiService _recipeApiService;

        public Search(RecipeApiService recipeApiService)
        {
            _recipeApiService = recipeApiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Hiển thị trang tìm kiếm ban đầu
        }

        [HttpPost]
        public async Task<IActionResult> Index(string query, int maxReadyTime = 45, int number = 10)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError("", "Tên món ăn hoặc nguyên liệu không được để trống.");
                return View();
            }

            try
            {
                // Gọi RecipeApiService để lấy kết quả tìm kiếm
                var jsonResponse = await _recipeApiService.GetRecipesAsync(query, null, null, null, maxReadyTime, number);

                // Parse kết quả JSON để lấy danh sách kết quả tìm kiếm
                var searchResults = JObject.Parse(jsonResponse)["results"];

                if (searchResults == null || !searchResults.HasValues)
                {
                    ModelState.AddModelError("", "Không tìm thấy công thức nào phù hợp với yêu cầu.");
                    return View();
                }

                // Tạo danh sách RecipeModel từ kết quả tìm kiếm
                var recipes = new List<RecipeModel>();
                foreach (var item in searchResults)
                {
                    var recipe = new RecipeModel
                    {
                        Title = item["title"]?.ToString(),
                        ImageUrl = item["image"]?.ToString(),
                        UsedIngredientCount = item["usedIngredientCount"]?.ToObject<int>() ?? 0,
                        MissedIngredientCount = item["missedIngredientCount"]?.ToObject<int>() ?? 0
                    };
                    recipes.Add(recipe);
                }

                // Trả về danh sách công thức tới view
                return View(recipes);
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API.");
                return View();
            }
        }


        public async Task<IActionResult> RecipeDetails(int id)
        {
            try
            {
                // Gọi RecipeApiService để lấy thông tin chi tiết món ăn
                var jsonResponse = await _recipeApiService.GetRecipeDetailsAsync(id);

                // Phân tích JSON để lấy thông tin món ăn
                var recipeDetails = JObject.Parse(jsonResponse);

                // Tạo mô hình cho món ăn
                var recipeModel = new RecipeModel
                {
                    Title = recipeDetails["title"]?.ToString() ?? "Không có tiêu đề", // Nếu null, gán một giá trị mặc định
                    ImageUrl = recipeDetails["image"]?.ToString() ?? "default_image_url.jpg", // Gán URL hình ảnh mặc định
                    Instructions = recipeDetails["instructions"]?.ToString() ?? "Không có hướng dẫn", // Nếu không có hướng dẫn
                    Ingredients = recipeDetails["extendedIngredients"]?.Select(i => i["name"]?.ToString()).ToList() ?? new List<string>(), // Gán danh sách nguyên liệu
                };

                // Trả về view với thông tin món ăn
                return View(recipeModel);
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API.");
                return View(new RecipeModel()); // Trả về một đối tượng RecipeModel mới nếu có lỗi
            }
        }


    }
}
