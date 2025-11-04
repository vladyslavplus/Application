using Evently.Application.DTOs.Event;

namespace Evently.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetPublicEventsAsync(Guid? currentUserId = null, CancellationToken cancellationToken = default);
        Task<EventDetailDto?> GetByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventDto>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<EventDto> CreateAsync(Guid organizerId, EventCreateDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid id, Guid organizerId, EventUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, Guid organizerId, bool isAdmin = false, CancellationToken cancellationToken = default);
        Task<bool> JoinAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> LeaveAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    }
}