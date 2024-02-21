namespace SoupProject.Models
{
    public class Category
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; } = string.Empty;
        public string categoryImg { get; set; } = string.Empty;
        public string categoryDesc { get; set; } = string.Empty;
        public int courseCount { get; set; }
        public string categoryStatus { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;
    }
}
