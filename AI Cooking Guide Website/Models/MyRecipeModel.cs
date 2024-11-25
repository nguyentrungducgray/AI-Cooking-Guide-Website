namespace AI_Cooking_Guide_Website.Models
{
    public class MyRecipeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
        public string Description { get; set; }
       
        public string Image { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        // Add the following properties:
        public string IngredientsText { get; set; }
        public string StepsText { get; set; }


        public bool IsAccepted { get; set; }

        // These will hold the parsed lists
        public List<string> Ingredients { get; set; }
        public List<string> Steps { get; set; }
    }
}
