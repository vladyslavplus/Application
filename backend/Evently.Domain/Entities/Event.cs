namespace Evently.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Location { get; set; } = default!;
        public int? Capacity { get; set; }
        public bool IsPublic { get; set; } = true;
        public Guid OrganizerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    }
}
