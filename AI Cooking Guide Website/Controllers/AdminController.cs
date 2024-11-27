using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Authorization;

namespace AI_Cooking_Guide_Website.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        [HttpPost("SaveRecipe")]
        public IActionResult SaveRecipe([FromForm] AddModel.Recipe recipe, IFormFile image)
        {
            try
            {
                // Kiểm tra xem có ảnh được chọn không
                if (image == null || image.Length == 0)
                {
                    return BadRequest("Hãy chọn ảnh để tải lên.");
                }

                // Lưu ảnh vào thư mục wwwroot/images
                var imagesDirectory = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory); // Tạo thư mục nếu chưa tồn tại
                }

                var imagePath = Path.Combine(imagesDirectory, image.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                // Lưu đường dẫn ảnh vào thuộc tính của món ăn
                recipe.Image = "/images/" + image.FileName; // Đường dẫn tới ảnh

                // Xử lý danh sách Ingredients
                if (recipe.Ingredients != null)
                {
                    recipe.Ingredients = CleanList(recipe.Ingredients);
                }

                // Xử lý danh sách Steps
                if (recipe.Steps != null)
                {
                    recipe.Steps = CleanList(recipe.Steps);
                }

                // Đường dẫn lưu tệp JSON
                var filePath = Path.Combine("wwwroot", "data", "recipes.json");
                List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

                // Đọc dữ liệu JSON hiện có
                if (System.IO.File.Exists(filePath))
                {
                    var jsonData = System.IO.File.ReadAllText(filePath);
                    recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>(); // Khởi tạo nếu null
                }

                // Tìm ID lớn nhất và gán ID mới
                int maxId = recipes.Any() ? recipes.Max(r => r.Id) : 0; // Lấy ID lớn nhất hoặc gán 0 nếu danh sách trống
                recipe.Id = maxId + 1;

                // Thêm món ăn mới vào danh sách
                recipes.Add(recipe);

                // Lưu danh sách vào tệp JSON
                var updatedData = JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true }); // Thêm WriteIndented để JSON dễ đọc
                System.IO.File.WriteAllText(filePath, updatedData);

                return Ok(recipe); // Trả về thông tin món ăn mới được lưu
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi: " + ex.Message);
            }
        }

        // Hàm làm sạch danh sách, xử lý JSON lồng nhau
        private List<string> CleanList(List<string> inputList)
        {
            List<string> cleanedList = new List<string>();

            foreach (var item in inputList)
            {
                // Kiểm tra nếu phần tử là chuỗi JSON lồng nhau
                if (item.StartsWith("[") && item.EndsWith("]"))
                {
                    try
                    {
                        // Giải mã chuỗi JSON thành danh sách
                        var deserializedItems = JsonSerializer.Deserialize<List<string>>(item);

                        // Thêm các phần tử đã giải mã vào danh sách chính nếu chưa tồn tại
                        foreach (var subItem in deserializedItems)
                        {
                            if (!cleanedList.Contains(subItem.Trim()))
                            {
                                cleanedList.Add(subItem.Trim());
                            }
                        }
                    }
                    catch
                    {
                        // Bỏ qua nếu chuỗi không thể giải mã
                        if (!cleanedList.Contains(item.Trim()))
                        {
                            cleanedList.Add(item.Trim());
                        }
                    }
                }
                else
                {
                    // Thêm phần tử thông thường (sau khi loại bỏ khoảng trắng)
                    if (!cleanedList.Contains(item.Trim()))
                    {
                        cleanedList.Add(item.Trim());
                    }
                }
            }

            return cleanedList;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
