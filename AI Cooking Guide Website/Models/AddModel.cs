namespace AI_Cooking_Guide_Website.Models
{
	public class AddModel
	{
		public List<Recipe> Recipes { get; set; }

		public class Recipe
		{
            public int Id { get; set; } // Id tự động tăng
            public string Name { get; set; }
			public List<string> Ingredients { get; set; }
			public string Description { get; set; }
			public List<string> Steps { get; set; }
			public string Image { get; set; }
		}
	}
}