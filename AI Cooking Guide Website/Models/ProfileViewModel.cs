namespace AI_Cooking_Guide_Website.Models
{
    public class ProfileViewModel
    {
        public Users User { get; set; }
        public List<MyRecipeModel> Recipes { get; set; }
        public List<AdminReplyModel> AdminReplies { get; set; }


    }
}
