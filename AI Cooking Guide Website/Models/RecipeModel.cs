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
        public RecipeModel KnowledgeGraph { get; set; } // Kết quả từ Knowledge Graph
        public List<RecipeModel> Organic { get; set; }  // Danh sách kết quả tìm kiếm

        public SearchResultModel()
        {
            Organic = new List<RecipeModel>();
        }
    }

    public class ImageSearchResultModel
    {
        public string Title { get; set; }          // Tiêu đề của kết quả
        public string ImageUrl { get; set; }       // URL ảnh gốc
        public string ThumbnailUrl { get; set; }   // URL ảnh thu nhỏ
        public string Source { get; set; }         // Nguồn bài viết
        public string Domain { get; set; }         // Tên miền
        public string Link { get; set; }           // Link đến bài viết
        public string GoogleUrl { get; set; }      // Google URL gốc
        public int Position { get; set; }          // Vị trí trong danh sách kết quả
    }
}
