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
        /// <summary>
        /// Tiêu đề của kết quả tìm kiếm hình ảnh
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// URL của hình ảnh lớn (kích thước gốc)
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// URL của hình ảnh thumbnail
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Nguồn cung cấp hình ảnh (vd: tên trang web)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Domain của trang web (vd: "example.com")
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Liên kết đến bài viết hoặc trang chứa hình ảnh
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Liên kết Google tìm kiếm liên quan đến hình ảnh
        /// </summary>
        public string GoogleUrl { get; set; }

        /// <summary>
        /// Vị trí của hình ảnh trong kết quả tìm kiếm
        /// </summary>
        public int Position { get; set; }
    }

}
