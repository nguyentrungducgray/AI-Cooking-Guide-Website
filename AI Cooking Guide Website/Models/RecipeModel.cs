namespace AI_Cooking_Guide_Website.Models
{
    public class RecipeModel
    {
        public string Title { get; set; }          // Tên món ăn
        public string Snippet { get; set; }       // Mô tả ngắn
        public string Link { get; set; }          // Link chi tiết
        public string ImageUrl { get; set; }      // URL ảnh
    }

    public class SearchResultModel
    {
        public List<RecipeModel> Organic { get; set; }  // Danh sách kết quả tìm kiếm
       

        public SearchResultModel()
        {
            Organic = new List<RecipeModel>();
           
        }
    }

    public class ImageSearchResultModel
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }

        public List<ImageSearchResultModel> Images { get; set; }
       

        public ImageSearchResultModel()
        {
            Images = new List<ImageSearchResultModel>();
            
        }
    }
}
