namespace SoupProject.DTOs
{
    public class CategoryDTO
    {
        public string categoryName { get; set; } = string.Empty;
        public string categoryDesc { get; set; } = string.Empty;
        public string categoryStatus { get; set; } = string.Empty;
        public IFormFile ImageFile { get; set; }
        
    }
}
