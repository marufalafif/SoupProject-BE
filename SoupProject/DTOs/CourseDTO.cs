namespace SoupProject.DTOs
{
    public class CourseDTO
    {
        public string courseName { get; set; } = string.Empty;
        public int categoryId { get; set; }
        public IFormFile courseImage { get; set; }
        public string courseDesc { get; set; } = string.Empty;
        public int coursePrice { get; set; }
      
        public string courseStatus { get; set; } = string.Empty;
    }
}
