using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic; // Để sử dụng List<T>
using System.IO; // Để sử dụng Path
using System.Linq; // Để sử dụng phương thức FirstOrDefault

namespace AI_Cooking_Guide_Website.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("ViewRecipes")]
        public IActionResult ViewRecipes()
        {
            var filePath = Path.Combine("wwwroot", "data", "recipes.json");
            List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

            // Đọc dữ liệu JSON hiện có
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }

            return View(recipes); // Trả về view với danh sách món ăn
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "recipes.json");
            List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

            // Đọc dữ liệu JSON hiện có
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }

            // Lấy 9 món mới nhất
            var latestRecipes = recipes.OrderByDescending(r => r.Id).Take(9).ToList();
            return View(latestRecipes); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Đã sửa phương thức FoodDetails để tìm món ăn theo id
        [HttpGet("FoodDetails/{id}")]
        public IActionResult FoodDetails(int id)
        {
            var filePath = Path.Combine("wwwroot", "data", "recipes.json");
            List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

            // Đọc dữ liệu JSON hiện có
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }

            // Tìm món ăn theo id
            var recipeDetail = recipes.FirstOrDefault(r => r.Id == id);
            if (recipeDetail == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy món ăn
            }

            return View(recipeDetail); // Trả về View với thông tin chi tiết món ăn
        }

        public IActionResult List(string searchKeyword)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "recipes.json");
            List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

            // Đọc dữ liệu JSON hiện có
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }

            // Nếu có từ khóa tìm kiếm, lọc món ăn theo tên chứa từ khóa đó
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                recipes = recipes.Where(r => r.Name.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(recipes); // Trả về view với danh sách món ăn
        }


        [HttpGet("MoreRecipes")]
        public IActionResult MoreRecipes()
        {
            var filePath = Path.Combine("wwwroot", "data", "recipes.json");
            List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

            // Đọc dữ liệu JSON hiện có
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>();
            }

            // Bỏ qua 9 món mới nhất và lấy phần còn lại
            var remainingRecipes = recipes.OrderByDescending(r => r.Id).Skip(9).ToList();
            return View(remainingRecipes);
        }


    }

}
