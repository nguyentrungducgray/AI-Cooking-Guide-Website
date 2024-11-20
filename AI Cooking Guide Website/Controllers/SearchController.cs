using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
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
                Organic = new List<RecipeModel>() // Danh sách mặc định trống
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string query)
        {
            // Lưu query vào ViewBag để hiển thị lại
            ViewBag.Query = query;
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError("", "Tên món ăn hoặc nguyên liệu không được để trống.");
                // Chuyển hướng tới GET method với từ khóa trong Query String
                return RedirectToAction("Index", new { query });
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

                // Danh sách các ảnh mặc định
                var placeholderImages = new List<string>
                {
                    "/imgUser/image1.jpg",
                    "/imgUser/image2.jpg",
                    "/imgUser/image3.jpg",
                    "/imgUser/image4.jpg",
                    "/imgUser/image5.jpg",
                    "/imgUser/image6.jpg",
                    "/imgUser/image7.jpg",
                    "/imgUser/image8.jpg",
                    "/imgUser/image9.jpg",
                    "/imgUser/image10.jpg",
                    "/imgUser/image11.jpg",
                    "/imgUser/image12.jpg",
                    "/imgUser/image13.jpg",
                    "/imgUser/image14.jpg",
                    "/imgUser/image15.jpg",
                    "/imgUser/image16.jpg",
                    "/imgUser/image17.jpg",
                    "/imgUser/image18.jpg",
                    "/imgUser/image19.jpg",
                    "/imgUser/image20.jpg",
                };

                // Tạo danh sách ảnh ngẫu nhiên
                var random = new Random();
                var shuffledImages = placeholderImages.OrderBy(_ => random.Next()).ToList();
                int currentIndex = 0;

                // Xử lý kết quả Organic (nếu có)
                var organicResults = searchResults["organic"];
                if (organicResults != null && organicResults.HasValues)
                {
                    foreach (var item in organicResults)
                    {
                        // Lấy ảnh tiếp theo
                        var imageUrl = item["imageUrl"]?.ToString();
                        if (string.IsNullOrWhiteSpace(imageUrl))
                        {
                            imageUrl = shuffledImages[currentIndex];
                            currentIndex = (currentIndex + 1) % shuffledImages.Count; // Quay vòng ảnh
                        }

                        var recipe = new RecipeModel
                        {
                            Title = item["title"]?.ToString(),
                            Snippet = item["snippet"]?.ToString(),
                            Link = item["link"]?.ToString(),
                            ImageUrl = imageUrl
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

        // Xử lý tìm kiếm theo hình ảnh
        [HttpPost]
        public async Task<IActionResult> SearchByImage(IFormFile imageQuery)
        {
            if (imageQuery == null || imageQuery.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng cung cấp hình ảnh để tìm kiếm.");
                return View("Index");
            }

            try
            {
                // Chuyển hình ảnh thành chuỗi base64 để gửi qua API
                using (var memoryStream = new MemoryStream())
                {
                    await imageQuery.CopyToAsync(memoryStream);
                    var imageBase64 = Convert.ToBase64String(memoryStream.ToArray());

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/images");
                    request.Headers.Add("X-API-KEY", "8c10c44a0cbc789d18974590233c7f4a2610ab50");

                    string contentJson = $"{{\"q\":\"{imageBase64}\",\"gl\":\"vn\",\"hl\":\"vi\"}}";
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
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API tìm kiếm hình ảnh.");
                return View("Index");
            }
        }
    }
}
