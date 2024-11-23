namespace AI_Cooking_Guide_Website.Models
{
    public class Users
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; } // For the random profile image
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

    }
}
