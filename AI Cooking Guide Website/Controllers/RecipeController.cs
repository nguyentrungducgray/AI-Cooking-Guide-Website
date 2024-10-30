using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
	public class RecipeController : Controller
	{
		[HttpPost("SaveRecipe")]
		public IActionResult SaveRecipe([FromForm] AddModel.Recipe recipe, IFormFile Image)
		{
			try
			{
				if (Image == null || Image.Length == 0)
				{
					return BadRequest("Hãy chọn ảnh để tải lên.");
				}

				// Đường dẫn lưu ảnh
				var imagePath = Path.Combine("wwwroot", "images", Image.FileName);
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					Image.CopyTo(stream);
				}

				recipe.Image = "/images/" + Image.FileName;

				// Đường dẫn lưu tệp JSON
				var filePath = Path.Combine("wwwroot", "data", "recipes.json");
				List<AddModel.Recipe> recipes = new List<AddModel.Recipe>();

				// Đọc dữ liệu JSON hiện có
				if (System.IO.File.Exists(filePath))
				{
					var jsonData = System.IO.File.ReadAllText(filePath);
					recipes = JsonSerializer.Deserialize<List<AddModel.Recipe>>(jsonData);
				}

				// Thêm món ăn mới và lưu vào tệp JSON
				recipes.Add(recipe);
				var updatedData = JsonSerializer.Serialize(recipes);
				System.IO.File.WriteAllText(filePath, updatedData);

				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Lỗi: " + ex.Message);
			}
		}
	}
}