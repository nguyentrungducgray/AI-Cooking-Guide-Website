using System.Net.Http;
using System.Threading.Tasks;

namespace AI_Cooking_Guide_Website.ModelAI
{
    public class RecipeApiService
    {
        private readonly HttpClient _httpClient;

        public RecipeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "7260c3ebffmsh774f1e9f8a617f6p14aff2jsn364a929dea9d");
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com");
        }

        public async Task<string> GetRecipesAsync(string query, string diet, string intolerances, string ingredients, int maxReadyTime, int number)
        {
            // Xây dựng URL động dựa trên các tham số đầu vào
            var requestUrl = $"https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/complexSearch?query={query}&diet={diet}&intolerances={intolerances}&includeIngredients={ingredients}&maxReadyTime={maxReadyTime}&number={number}&instructionsRequired=true&fillIngredients=false&addRecipeInformation=false&addRecipeInstructions=false&addRecipeNutrition=false&ignorePantry=true&sort=max-used-ingredients";

            // Thực hiện yêu cầu API
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // lấy chi tiết công thức dựa trên ID món ăn 

        public async Task<string> GetRecipeDetailsAsync(int id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/{id}/information"),
                Headers =
        {
            { "x-rapidapi-key", "7260c3ebffmsh774f1e9f8a617f6p14aff2jsn364a929dea9d" },
            { "x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
