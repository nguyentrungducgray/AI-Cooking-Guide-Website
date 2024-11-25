using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Newtonsoft.Json;
using System.Text;

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



        // Xử lý tìm kiếm theo hình ảnh

        [HttpPost]
        public async Task<IActionResult> SearchByImageAI(IFormFile imageQuery)
        {
            if (imageQuery == null || imageQuery.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng cung cấp hình ảnh để tìm kiếm.");
                return View("Index", new SearchResultModel());
            }

            try
            {
                // Chuyển hình ảnh thành chuỗi base64
                string imageBase64;
                using (var memoryStream = new MemoryStream())
                {
                    await imageQuery.CopyToAsync(memoryStream);
                    imageBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }

                // Gửi yêu cầu đến API tìm kiếm hình ảnh
                var request = new HttpRequestMessage(HttpMethod.Post, "https://your-ai-service.com/api/search");
                request.Headers.Add("Authorization", "Bearer YOUR_API_KEY");

                var payload = new
                {
                    image = imageBase64,
                    options = new { gl = "vn", hl = "vi" }
                };
                string contentJson = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(contentJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResults = JObject.Parse(jsonResponse);

                // Xử lý kết quả trả về
                var imageResults = new List<ImageSearchResultModel>();
                var imageItems = searchResults["results"];
                foreach (var item in imageItems)
                {
                    imageResults.Add(new ImageSearchResultModel
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

                // Truyền kết quả vào mô hình và trả về view Index
                var model = new SearchResultModel
                {
                    Organic = new List<RecipeModel>(), // Giữ danh sách Organic trống nếu không tìm kiếm văn bản
                };

                ViewBag.ImageResults = imageResults; // Đặt kết quả hình ảnh vào ViewBag
                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi tìm kiếm bằng AI: {ex.Message}");
                return View("Index", new SearchResultModel());
            }
        }


    }
}
