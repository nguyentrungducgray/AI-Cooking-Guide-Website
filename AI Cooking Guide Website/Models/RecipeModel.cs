namespace AI_Cooking_Guide_Website.Models
{
    public class RecipeModel
    {
        public int Id { get; set; } // Thêm thuộc tính ID
        public string Title { get; set; }
        public string ImageUrl { get; set; }
       
        public int UsedIngredientCount { get; set; }
        public int MissedIngredientCount { get; set; }
    }

}
