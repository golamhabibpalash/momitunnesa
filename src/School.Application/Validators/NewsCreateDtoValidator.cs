using FluentValidation;
using School.Application.DTOs.News;

namespace School.Application.Validators;

public class NewsCreateDtoValidator : AbstractValidator<NewsCreateDto>
{
    public NewsCreateDtoValidator()
    {
        RuleFor(x => x.TitleEn)
            .NotEmpty().WithMessage("English title is required.")
            .MaximumLength(500).WithMessage("English title must not exceed 500 characters.");

        RuleFor(x => x.TitleBn)
            .NotEmpty().WithMessage("Bangla title is required.")
            .MaximumLength(500).WithMessage("Bangla title must not exceed 500 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid news category.");
    }
}
