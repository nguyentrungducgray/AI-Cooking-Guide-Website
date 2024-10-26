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
                        Id = item["id"]?.ToObject<int>() ?? 0,  // Lấy ID
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


        [HttpGet]
        public async Task<IActionResult> RecipeDetails(int id)
        {
            try
            {
                var jsonResponse = await _recipeApiService.GetRecipeDetailsAsync(id);
                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    // Nếu không có phản hồi từ API, trả về một view với thông báo lỗi
                    ModelState.AddModelError("", "Không tìm thấy thông tin món ăn.");
                    return View(new RecipeDetailModel());
                }

                var recipeDetails = JObject.Parse(jsonResponse);
                var recipeModel = new RecipeDetailModel
                {
                    Id = recipeDetails["id"].Value<int>(),
                    Title = recipeDetails["title"]?.ToString(),
                    ReadyInMinutes = recipeDetails["readyInMinutes"]?.Value<int>() ?? 0,
                    Servings = recipeDetails["servings"]?.Value<int>() ?? 0,
                    SourceUrl = recipeDetails["sourceUrl"]?.ToString(),
                    Image = recipeDetails["image"]?.ToString(),
                    Summary = recipeDetails["summary"]?.ToString(),
                    DishTypes = recipeDetails["dishTypes"]?.ToObject<List<string>>() ?? new List<string>(),
                    Diets = recipeDetails["diets"]?.ToObject<List<string>>() ?? new List<string>(),
                    Instructions = recipeDetails["instructions"]?.ToString(),
                    SpoonacularScore = recipeDetails["spoonacularScore"]?.Value<double>() ?? 0.0,
                    AnalyzedInstructions = recipeDetails["analyzedInstructions"]?.Select(ai => new AnalyzedInstructionModel
                    {
                        Name = ai["name"]?.ToString(),
                        Steps = ai["steps"]?.Select(s => new StepModel
                        {
                            Number = s["number"]?.ToObject<int>() ?? 0,
                            Step = s["step"]?.ToString(),
                            Ingredients = s["ingredients"]?.Select(i => new IngredientModel
                            {
                                Id = i["id"]?.ToObject<int>() ?? 0,
                                Name = i["name"]?.ToString(),
                                LocalizedName = i["localizedName"]?.ToString(),
                                Image = i["image"]?.ToString()
                            }).ToList() ?? new List<IngredientModel>()
                        }).ToList() ?? new List<StepModel>()
                    }).ToList() ?? new List<AnalyzedInstructionModel>()

                };

                return View(recipeModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API: " + ex.Message);
                return View(new RecipeDetailModel());
            }
        }



    }
}
