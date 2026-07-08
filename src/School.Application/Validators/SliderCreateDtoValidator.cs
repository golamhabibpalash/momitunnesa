using FluentValidation;
using School.Application.DTOs.Slider;

namespace School.Application.Validators;

public class SliderCreateDtoValidator : AbstractValidator<SliderCreateDto>
{
    public SliderCreateDtoValidator()
    {
        RuleFor(x => x.TitleEn)
            .MaximumLength(500).WithMessage("English title must not exceed 500 characters.");

        RuleFor(x => x.TitleBn)
            .MaximumLength(500).WithMessage("Bangla title must not exceed 500 characters.");

        RuleFor(x => x.SubtitleEn)
            .MaximumLength(1000).WithMessage("English subtitle must not exceed 1000 characters.");

        RuleFor(x => x.SubtitleBn)
            .MaximumLength(1000).WithMessage("Bangla subtitle must not exceed 1000 characters.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.LinkUrl)
            .MaximumLength(500).WithMessage("Link URL must not exceed 500 characters.");
    }
}
