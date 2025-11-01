using Evently.Domain.Entities;
using Evently.Domain.Interfaces;
using Evently.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Evently.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetPublicEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.IsPublic)
            .Include(e => e.Participants)
            .OrderBy(e => e.StartDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.OrganizerId == userId || e.Participants.Any(p => p.UserId == userId))
            .Include(e => e.Participants)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Participants)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(Event? Event, Dictionary<Guid, string> UserNames)> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ev = await _context.Events
            .Include(e => e.Participants)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (ev == null)
            return (null, new Dictionary<Guid, string>());

        var participantIds = ev.Participants.Select(p => p.UserId).ToList();

        var userNames = await _context.Users
            .Where(u => participantIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName })
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        return (ev, userNames);
    }

    public async Task AddAsync(Event entity, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event entity, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Event entity, CancellationToken cancellationToken = default)
    {
        _context.Events.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> JoinEventAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
        if (ev == null)
            return false;

        if (ev.Capacity.HasValue)
        {
            var count = await _context.EventParticipants
                .CountAsync(p => p.EventId == eventId, cancellationToken);

            if (count >= ev.Capacity.Value)
                return false;
        }

        var alreadyJoined = await _context.EventParticipants
            .AnyAsync(p => p.EventId == eventId && p.UserId == userId, cancellationToken);
        if (alreadyJoined)
            return false;

        await _context.EventParticipants.AddAsync(new EventParticipant
        {
            EventId = eventId,
            UserId = userId,
            JoinedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> LeaveEventAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var participant = await _context.EventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId, cancellationToken);

        if (participant == null)
            return false;

        _context.EventParticipants.Remove(participant);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}