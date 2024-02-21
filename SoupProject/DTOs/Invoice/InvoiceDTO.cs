namespace SoupProject.DTOs.Invoice
{
    public class InvoiceDTO
    {
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string invoice {  get; set; } = string.Empty;
        public DateTime transactionDate { get; set; }
        public int totalCourse { get; set; }
        public int totalPrice { get; set; }
    }
}
