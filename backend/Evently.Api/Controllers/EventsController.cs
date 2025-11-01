using Evently.Application.DTOs.Event;
using Evently.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evently.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Get all public events.
        /// </summary>
        /// <returns>List of public events.</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicEvents(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetPublicEventsAsync(cancellationToken);
            return Ok(events);
        }

        /// <summary>
        /// Get event by ID.
        /// </summary>
        /// <param name="id">Event ID.</param>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var ev = await _eventService.GetByIdAsync(id, cancellationToken);
            if (ev == null)
                return NotFound();

            return Ok(ev);
        }

        /// <summary>
        /// Get all events where the current user is an organizer or participant.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserEvents(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var events = await _eventService.GetUserEventsAsync(userId, cancellationToken);
            return Ok(events);
        }

        /// <summary>
        /// Create a new event (only for logged-in users).
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto, CancellationToken cancellationToken)
        {
            var organizerId = GetUserId();

            var created = await _eventService.CreateAsync(organizerId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing event (only the organizer can edit).
        /// </summary>
        [HttpPatch("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventUpdateDto dto, CancellationToken cancellationToken)
        {
            var organizerId = GetUserId();

            var success = await _eventService.UpdateAsync(id, organizerId, dto, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete an event (organizer or admin can delete).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var success = await _eventService.DeleteAsync(id, userId, isAdmin, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Join an event as a participant.
        /// </summary>
        [HttpPost("{id:guid}/join")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Join(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var success = await _eventService.JoinAsync(id, userId, cancellationToken);
            if (!success)
                return BadRequest("Unable to join event (maybe full or already joined).");

            return Ok(new { Message = "Joined successfully" });
        }

        /// <summary>
        /// Leave an event.
        /// </summary>
        [HttpPost("{id:guid}/leave")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Leave(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var success = await _eventService.LeaveAsync(id, userId, cancellationToken);
            if (!success)
                return BadRequest("You are not part of this event.");

            return Ok(new { Message = "Left successfully" });
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
        }
    }
}