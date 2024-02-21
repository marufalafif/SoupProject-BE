namespace SoupProject.Models
{
    public class Schedule
    {
        public int scheduleId { get; set; }
        public Guid courseId { get; set; }
        public string startTime { get; set; } = string.Empty;
        public int isActive { get; set; }
    }
}
