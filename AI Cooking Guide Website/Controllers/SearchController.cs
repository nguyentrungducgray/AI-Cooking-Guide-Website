using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class SearchController : Controller
    {
        private readonly HttpClient _httpClient;

        public SearchController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new SearchResultModel
            {
                Organic = new List<RecipeModel>() // Default empty list
            };
            return View(model);
           
        }

        [HttpPost]
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError("", "Tên món ăn hoặc nguyên liệu không được để trống.");
                return View();
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/search");
                request.Headers.Add("X-API-KEY", "8c10c44a0cbc789d18974590233c7f4a2610ab50");

                string refinedQuery = $"{query} công thức OR nấu ăn OR nguyên liệu OR ẩm thực";
                string contentJson = $"{{\"q\":\"{refinedQuery}\",\"gl\":\"vn\",\"hl\":\"vi\"}}";
                var content = new StringContent(contentJson, null, "application/json");
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResults = JObject.Parse(jsonResponse);

                var model = new SearchResultModel();
                // Handle Organic Results (if exists)
                var organicResults = searchResults["organic"];
                if (organicResults != null && organicResults.HasValues)
                {
                    foreach (var item in organicResults)
                    {
                        var recipe = new RecipeModel
                        {
                            Title = item["title"]?.ToString(),
                            Snippet = item["snippet"]?.ToString(),
                            Link = item["link"]?.ToString(),
                            ImageUrl = item["imageUrl"]?.ToString() ?? "/images/placeholder.jpg"
                        };

                        model.Organic.Add(recipe);
                    }
                }

                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult RecipeDetails(string title)
        {
            ModelState.AddModelError("", "Chức năng xem chi tiết không khả dụng.");
            return View(); // Trả về một thông báo cơ bản do không có API chi tiết.
        }

        [HttpPost]
        public async Task<IActionResult> SearchByImage(string imageQuery)
        {
            if (string.IsNullOrWhiteSpace(imageQuery))
            {
                ModelState.AddModelError("", "Vui lòng cung cấp từ khóa hoặc thông tin liên quan đến hình ảnh.");
                return View("Index");
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/images");
                request.Headers.Add("X-API-KEY", "8c10c44a0cbc789d18974590233c7f4a2610ab50");
                
                string contentJson = $"{{\"q\":\"{imageQuery}\",\"gl\":\"vn\",\"hl\":\"vi\"}}";
                var content = new StringContent(contentJson, null, "application/json");
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var imageResults = JObject.Parse(jsonResponse);

                var model = new List<ImageSearchResultModel>();

                var imageItems = imageResults["images_results"];
                if (imageItems != null && imageItems.HasValues)
                {
                    foreach (var item in imageItems)
                    {
                        model.Add(new ImageSearchResultModel
                        {
                            Title = item["title"]?.ToString(),
                            ImageUrl = item["imageUrl"]?.ToString(),
                            ThumbnailUrl = item["thumbnailUrl"]?.ToString(),
                            Source = item["source"]?.ToString(),
                            Domain = item["domain"]?.ToString(),
                            Link = item["link"]?.ToString(),
                            GoogleUrl = item["googleUrl"]?.ToString(),
                            Position = item["position"]?.ToObject<int>() ?? 0
                        });
                    }
                }

                return View("ImageResults", model); // Hiển thị kết quả tìm kiếm hình ảnh
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API tìm kiếm hình ảnh.");
                return View("Index");
            }
        }

    }
}
