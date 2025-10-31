using Evently.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Evently.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<EventParticipant> ParticipatingEvents { get; set; } = new List<EventParticipant>();
    }
}
