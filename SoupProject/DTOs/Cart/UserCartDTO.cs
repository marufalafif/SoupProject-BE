namespace SoupProject.DTOs.Cart
{
    public class UserCartDTO
    {
        public int cartId { get; set; }
        public string categoryName { get; set; }
        public string courseName { get; set; }
        public string courseDate { get; set; }
        public decimal coursePrice { get; set; }
        public bool isPaid { get; set; }
        public string courseImg { get; set; }
    }
}