using Evently.Application.DTOs.Event;
using FluentValidation;

namespace Evently.Application.Validators.Event
{
    public class EventUpdateValidator : AbstractValidator<EventUpdateDto>
    {
        public EventUpdateValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200)
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => x.Description != null);

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .When(x => x.Location != null);

            RuleFor(x => x.StartDate)
                .Must(date => date == null || date > DateTimeOffset.UtcNow)
                .WithMessage("Start date must be in the future");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .When(x => x.Capacity.HasValue)
                .WithMessage("Capacity must be positive");
        }
    }
}