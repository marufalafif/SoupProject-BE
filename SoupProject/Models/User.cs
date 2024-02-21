namespace SoupProject.Models
{
    public class User
    {
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActivated { get; set; }
    }
}
