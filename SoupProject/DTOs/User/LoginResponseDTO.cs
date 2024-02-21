namespace SoupProject.DTOs.User
{
    public class LoginResponseDTO
    {
        public Guid userId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
