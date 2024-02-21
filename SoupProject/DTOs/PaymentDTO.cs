namespace SoupProject.DTOs
{
    public class PaymentDTO
    {
        public string paymentName { get; set; } = string.Empty;
        public string paymentStatus { get; set; } = string.Empty;
        public IFormFile ImageFile { get; set; }
    }
}
