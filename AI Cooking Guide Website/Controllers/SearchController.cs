using AI_Cooking_Guide_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
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
        public IActionResult Index(string query, string imageUrl)
        {
            var textSearchModel = new SearchResultModel
            {
                Organic = new List<RecipeModel>()
            };

            var imageSearchModel = new ImageSearchResultModel
            {
                Images = new List<ImageSearchResultModel>()
            };

            if (!string.IsNullOrEmpty(query))
            {
                var sessionKey = $"SearchResults_{query}";
                if (HttpContext.Session.TryGetValue(sessionKey, out var cachedData))
                {
                    textSearchModel = JsonConvert.DeserializeObject<SearchResultModel>(Encoding.UTF8.GetString(cachedData));
                }
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                var sessionKey = $"SearchResults_{imageUrl}";
                if (HttpContext.Session.TryGetValue(sessionKey, out var cachedData))
                {
                    imageSearchModel = JsonConvert.DeserializeObject<ImageSearchResultModel>(Encoding.UTF8.GetString(cachedData));
                }
            }

            ViewBag.Query = query;
            ViewBag.ImageUrl = imageUrl;

            // Determine which view to return based on input
            if (!string.IsNullOrEmpty(query))
            {
                return View("Index", textSearchModel); // Render text-based search results
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                return View("Index", imageSearchModel); // Render image-based search results
            }

            return View("Index", textSearchModel); // Default to text-based search
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query, string imageUrl)
        {
            if (string.IsNullOrEmpty(query) && string.IsNullOrEmpty(imageUrl))
            {
                ModelState.AddModelError("", "Vui lòng cung cấp từ khóa hoặc URL hình ảnh để tìm kiếm.");
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(query))
            {
                return await PerformTextSearch(query);
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                return await PerformImageSearch(imageUrl);
            }

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> PerformTextSearch(string query)
        {
            var sessionKey = $"SearchResults_{query}";
            if (HttpContext.Session.TryGetValue(sessionKey, out var cachedData))
            {
                return RedirectToAction("Index", new { query });
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/search");
                request.Headers.Add("X-API-KEY", "8c10c44a0cbc789d18974590233c7f4a2610ab50");

                string refinedQuery = $"{query} công thức OR nấu ăn OR nguyên liệu OR ẩm thực";
                string contentJson = $"{{\"q\":\"{refinedQuery}\",\"gl\":\"vn\",\"hl\":\"vi\", \"num\":20}}";
                var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
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
                        model.Organic.Add(new RecipeModel
                        {
                            Title = item["title"]?.ToString(),
                            Snippet = item["snippet"]?.ToString(),
                            Link = item["link"]?.ToString(),
                            ImageUrl = imageUrl,
                        });
                    }
                }

                HttpContext.Session.Set(sessionKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
                return RedirectToAction("Index", new { query });
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API cho tìm kiếm văn bản.");
                return RedirectToAction("Index");
            }
        }

        private async Task<IActionResult> PerformImageSearch(string imageUrl)
        {
            // Validate the provided image URL
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                ModelState.AddModelError("", "URL hình ảnh không hợp lệ.");
                return RedirectToAction("Index");
            }

            var sessionKey = $"SearchResults_{imageUrl}";

            // Check if the result is already cached
            if (HttpContext.Session.TryGetValue(sessionKey, out var cachedData))
            {
                return RedirectToAction("Index", new { imageUrl });
            }

            try
            {
                // Create HTTP request for the image search API
                var request = new HttpRequestMessage(HttpMethod.Post, "https://google.serper.dev/lens");
                request.Headers.Add("X-API-KEY", "8c10c44a0cbc789d18974590233c7f4a2610ab50");
                string refinedQuery = $"công thức OR nấu ăn OR nguyên liệu OR ẩm thực";
                // Prepare payload with image URL
                var payload = new
                {
                    url = imageUrl,
                    gl = "vn",
                    hl = "vi"
                };

                // Add payload to request body
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                // Send the request
                var response = await _httpClient.SendAsync(request);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Parse the response JSON
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResults = JObject.Parse(jsonResponse);

                // Process search results into the model
                var model = new ImageSearchResultModel();
                var imageResults = searchResults["organic"];

                if (imageResults != null && imageResults.HasValues)
                {
                    foreach (var item in imageResults)
                    {
                        model.Images.Add(new ImageSearchResultModel
                        {
                            Title = item["title"]?.ToString(),
                            Source = item["source"]?.ToString(),
                            Link = item["link"]?.ToString(),
                            ImageUrl = item["imageUrl"]?.ToString()
                        });
                    }
                }

                // Cache the results in the session
                HttpContext.Session.Set(sessionKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));

                // Redirect to display the results
                return RedirectToAction("Index", new { imageUrl });
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"HTTP Request Error: {ex.Message}");

                ModelState.AddModelError("", "Có lỗi xảy ra khi gọi API cho tìm kiếm hình ảnh.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"General Error: {ex.Message}");

                ModelState.AddModelError("", "Một lỗi không xác định đã xảy ra.");
                return RedirectToAction("Index");
            }
        }

    }
}