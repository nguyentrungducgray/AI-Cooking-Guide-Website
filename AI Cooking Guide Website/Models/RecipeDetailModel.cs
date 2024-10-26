namespace AI_Cooking_Guide_Website.Models
{
    public class RecipeDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public string SourceUrl { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
        public List<string> DishTypes { get; set; }
        public List<string> Diets { get; set; }
        public string Instructions { get; set; }
        public List<AnalyzedInstructionModel> AnalyzedInstructions { get; set; } // Thay đổi ở đây
        public double SpoonacularScore { get; set; }
    }
    public class AnalyzedInstructionModel
    {
        public string Name { get; set; } // Tên có thể có (nếu cần thiết)
        public List<StepModel> Steps { get; set; } // Danh sách các bước thực hiện
    }

    public class StepModel
    {
        public int Number { get; set; }
        public string Step { get; set; }
        public List<IngredientModel> Ingredients { get; set; }
    }

    public class IngredientModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string Image { get; set; }
    }
}
