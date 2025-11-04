using Evently.Application.DTOs.Event;
using Evently.Application.Interfaces;
using Evently.Domain.Entities;
using Evently.Domain.Interfaces;

namespace Evently.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EventDto>> GetPublicEventsAsync(Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var events = await _repository.GetPublicEventsAsync(cancellationToken);
            return events.Select(e => MapToDto(e, currentUserId));
        }

        public async Task<EventDetailDto?> GetByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var (ev, userNames) = await _repository.GetByIdWithUsersAsync(id, cancellationToken);
            if (ev == null)
                return null;

            return MapToDetailDto(ev, userNames, currentUserId);
        }

        public async Task<IEnumerable<EventDto>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var events = await _repository.GetUserEventsAsync(userId, cancellationToken);
            return events.Select(e => MapToDto(e, userId));
        }

        public async Task<EventDto> CreateAsync(Guid organizerId, EventCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                Capacity = dto.Capacity,
                IsPublic = dto.IsPublic,
                OrganizerId = organizerId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity, cancellationToken);
            return MapToDto(entity, organizerId);
        }

        public async Task<bool> UpdateAsync(Guid id, Guid organizerId, EventUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var ev = await _repository.GetByIdAsync(id, cancellationToken);
            if (ev == null || ev.OrganizerId != organizerId)
                return false;

            if (dto.Title != null) ev.Title = dto.Title;
            if (dto.Description != null) ev.Description = dto.Description;
            if (dto.StartDate.HasValue) ev.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) ev.EndDate = dto.EndDate.Value;
            if (dto.Location != null) ev.Location = dto.Location;
            if (dto.Capacity.HasValue)
                ev.Capacity = dto.Capacity.Value;
            else if (dto.Capacity == null)
                ev.Capacity = null;
            if (dto.IsPublic.HasValue) ev.IsPublic = dto.IsPublic.Value;

            await _repository.UpdateAsync(ev, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid organizerId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            var ev = await _repository.GetByIdAsync(id, cancellationToken);
            if (ev == null)
                return false;

            if (ev.OrganizerId != organizerId && !isAdmin)
                return false;

            await _repository.DeleteAsync(ev, cancellationToken);
            return true;
        }

        public async Task<bool> JoinAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _repository.JoinEventAsync(eventId, userId, cancellationToken);
        }

        public async Task<bool> LeaveAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _repository.LeaveEventAsync(eventId, userId, cancellationToken);
        }

        private static EventDto MapToDto(Event e, Guid? currentUserId = null)
        {
            return new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Capacity = e.Capacity,
                IsPublic = e.IsPublic,
                OrganizerId = e.OrganizerId,
                ParticipantCount = e.Participants?.Count ?? 0,
                IsJoined = currentUserId != null && e.Participants?.Any(p => p.UserId == currentUserId) == true
            };
        }

        private static EventDetailDto MapToDetailDto(Event e, Dictionary<Guid, string> userNames, Guid? currentUserId = null)
        {
            return new EventDetailDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Location = e.Location,
                Capacity = e.Capacity,
                IsPublic = e.IsPublic,
                OrganizerId = e.OrganizerId,
                ParticipantCount = e.Participants?.Count ?? 0,
                Participants = e.Participants?
                    .Select(p => new ParticipantDto
                    {
                        UserId = p.UserId,
                        FullName = userNames.ContainsKey(p.UserId)
                            ? userNames[p.UserId]
                            : "Unknown User"
                    }).ToList() ?? new List<ParticipantDto>(),
                IsJoined = currentUserId != null && e.Participants?.Any(p => p.UserId == currentUserId) == true
            };
        }
    }
}