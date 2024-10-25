﻿namespace AI_Cooking_Guide_Website.Models
{
    public class RecipeModel
    {
        public int Id { get; set; } // Thêm thuộc tính ID
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Ingredients { get; set; } // Thêm danh sách nguyên liệu
        public string Instructions { get; set; } // Thêm hướng dẫn
        public int UsedIngredientCount { get; set; }
        public int MissedIngredientCount { get; set; }
    }

}
