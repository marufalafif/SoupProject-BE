namespace SoupProject.DTOs.Cart
{
    public class CheckoutByCourseDTO
    {
        public Guid userId { get; set; }
        public string paymentMethod { get; set; } = string.Empty;
        public Guid courseId { get; set; }
        public string? courseDate { get; set; }
    }
}
