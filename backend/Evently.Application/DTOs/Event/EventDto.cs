namespace Evently.Application.DTOs.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Location { get; set; } = default!;
        public int? Capacity { get; set; }
        public bool IsPublic { get; set; }
        public int ParticipantCount { get; set; }
        public Guid OrganizerId { get; set; }
    }
}
