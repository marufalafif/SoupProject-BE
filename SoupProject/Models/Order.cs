namespace SoupProject.Models
{
    public class Order
    {
        public Guid orderId { get; set; }
        public string invoice { get; set; }
        public Guid userId { get; set; }
        public DateTime transactionDate { get; set; }
        public string paymentMethod { get; set; }
        public bool isPaid { get; set; }
        public int totalPrice { get; set; }
        public string courseDate { get; set; } 
    }
}