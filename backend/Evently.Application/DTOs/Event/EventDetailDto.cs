namespace Evently.Application.DTOs.Event
{
    public class EventDetailDto : EventDto
    {
        public List<ParticipantDto> Participants { get; set; } = new();
    }
}
