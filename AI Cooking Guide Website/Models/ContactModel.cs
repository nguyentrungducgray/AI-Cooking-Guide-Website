namespace AI_Cooking_Guide_Website.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
       
    } 
}
