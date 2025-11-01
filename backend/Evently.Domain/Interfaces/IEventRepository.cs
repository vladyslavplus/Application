using Evently.Domain.Entities;

namespace Evently.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetPublicEventsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(Event? Event, Dictionary<Guid, string> UserNames)> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Event entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Event entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Event entity, CancellationToken cancellationToken = default);
        Task<bool> JoinEventAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> LeaveEventAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    }
}
