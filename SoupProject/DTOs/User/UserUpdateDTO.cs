namespace SoupProject.DTOs.User
{
    public class UserUpdateDTO
    {
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActivated { get; set; }
    }
}
