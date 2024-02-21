namespace SoupProject.DTOs.Invoice
{
    public class InvoiceDetailDTO
    {
        public Guid userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string courseName {  get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public DateTime transactionDate { get; set; }
        public string schedule {  get; set; }
        public int coursePrice { get; set; }
    }
}
