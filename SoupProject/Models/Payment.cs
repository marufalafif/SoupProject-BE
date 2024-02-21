namespace SoupProject.Models
{
    public class Payment
    {
        public int paymentId { get; set; }
        public string paymentName { get; set; } = string.Empty;
        public string paymentImg { get; set; } = string.Empty;
        public string paymentStatus { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
