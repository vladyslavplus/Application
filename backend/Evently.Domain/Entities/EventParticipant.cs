namespace Evently.Domain.Entities
{
    public class EventParticipant
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; } = default!;
        public Guid UserId { get; set; }
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
