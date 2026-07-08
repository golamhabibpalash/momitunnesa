using FluentValidation;
using School.Application.DTOs.Event;

namespace School.Application.Validators;

public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
{
    public EventCreateDtoValidator()
    {
        RuleFor(x => x.TitleEn)
            .NotEmpty().WithMessage("English title is required.")
            .MaximumLength(500).WithMessage("English title must not exceed 500 characters.");

        RuleFor(x => x.TitleBn)
            .NotEmpty().WithMessage("Bangla title is required.")
            .MaximumLength(500).WithMessage("Bangla title must not exceed 500 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Event date is required.");

        RuleFor(x => x.Location)
            .MaximumLength(500).WithMessage("Location must not exceed 500 characters.");
    }
}
