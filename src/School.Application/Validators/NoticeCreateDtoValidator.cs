using FluentValidation;
using School.Application.DTOs.Notice;

namespace School.Application.Validators;

public class NoticeCreateDtoValidator : AbstractValidator<NoticeCreateDto>
{
    public NoticeCreateDtoValidator()
    {
        RuleFor(x => x.TitleEn)
            .NotEmpty().WithMessage("English title is required.")
            .MaximumLength(500).WithMessage("English title must not exceed 500 characters.");

        RuleFor(x => x.TitleBn)
            .NotEmpty().WithMessage("Bangla title is required.")
            .MaximumLength(500).WithMessage("Bangla title must not exceed 500 characters.");

        RuleFor(x => x.NoticeCategory)
            .IsInEnum().WithMessage("Invalid notice category.");

        RuleFor(x => x.AttachmentUrl)
            .MaximumLength(500).WithMessage("Attachment URL must not exceed 500 characters.");

        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("Publish date is required.");
    }
}
