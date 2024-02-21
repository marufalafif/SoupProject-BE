namespace SoupProject.DTOs.Cart
{
    public class CheckoutDTO
    {
        public Guid userId { get; set; }
        public string paymentMethod { get; set; } = string.Empty;
        public int[] selectedCourses { get; set; }
    }
}
