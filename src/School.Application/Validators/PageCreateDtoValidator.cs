using FluentValidation;
using School.Application.DTOs.Page;

namespace School.Application.Validators;

public class PageCreateDtoValidator : AbstractValidator<PageCreateDto>
{
    public PageCreateDtoValidator()
    {
        RuleFor(x => x.TitleEn)
            .MaximumLength(500).WithMessage("English title must not exceed 500 characters.");

        RuleFor(x => x.TitleBn)
            .MaximumLength(500).WithMessage("Bangla title must not exceed 500 characters.");

        RuleFor(x => x.FeaturedImage)
            .MaximumLength(500).WithMessage("Featured image URL must not exceed 500 characters.");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(200).WithMessage("Meta title must not exceed 200 characters.");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Meta description must not exceed 500 characters.");

        RuleFor(x => x.MetaKeywords)
            .MaximumLength(300).WithMessage("Meta keywords must not exceed 300 characters.");

        RuleFor(x => x.TemplateType)
            .IsInEnum().WithMessage("Invalid template type.");
    }
}
