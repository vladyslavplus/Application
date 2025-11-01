namespace Evently.Application.DTOs.Event
{
    public class EventUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public bool? IsPublic { get; set; }
    }
}
