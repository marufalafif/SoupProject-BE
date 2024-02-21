namespace SoupProject.Models
{
    public class Course
    {
        public Guid courseId { get; set; }
        public string courseName { get; set; } = string.Empty;
        public int categoryId { get; set; }
        public string courseImg { get; set; } = string.Empty;
        public string courseDesc { get; set; } = string.Empty;
        public int coursePrice { get; set; }
        public int CartCount { get; set; }

        public string courseStatus { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string categoryName { get; set; } = string.Empty;
    }
}
