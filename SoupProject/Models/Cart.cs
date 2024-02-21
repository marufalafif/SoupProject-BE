namespace SoupProject.Models
{
    public class Cart
    {
        public int cartId { get; set; }
        public Guid courseId { get; set; }
        public Guid userId { get; set; }
        public string courseDate { get; set; }
        public bool isPaid { get; set; }
    }
}
