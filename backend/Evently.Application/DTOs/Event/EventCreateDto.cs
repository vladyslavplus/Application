namespace Evently.Application.DTOs.Event
{
    public class EventCreateDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Location { get; set; } = default!;
        public int? Capacity { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
