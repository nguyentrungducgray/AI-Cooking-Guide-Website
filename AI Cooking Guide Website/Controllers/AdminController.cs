
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
	[Route("Admin")]
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

				// Đường dẫn lưu tệp JSON
				var filePath = Path.Combine("wwwroot", "data", "recipes.json");
				List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

				// Đọc dữ liệu JSON hiện có
				if (System.IO.File.Exists(filePath))
				{
					var jsonData = System.IO.File.ReadAllText(filePath);
					recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData) ?? new List<AddModel.Recipe>(); // Khởi tạo nếu null
				}

				// Thêm món ăn mới vào danh sách
				recipes.Add(recipe);

				// Lưu danh sách vào tệp JSON
				var updatedData = JsonSerializer.Serialize(recipes);
				System.IO.File.WriteAllText(filePath, updatedData);

				return Ok(recipe); // Trả về thông tin món ăn mới được lưu
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Lỗi: " + ex.Message);
			}
		}


		public IActionResult Index()
		{
			return View();
		}
	}
}
