using FluentValidation;
using School.Application.DTOs.Teacher;

namespace School.Application.Validators;

public class TeacherCreateDtoValidator : AbstractValidator<TeacherCreateDto>
{
    public TeacherCreateDtoValidator()
    {
        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required.")
            .MaximumLength(200).WithMessage("English name must not exceed 200 characters.");

        RuleFor(x => x.NameBn)
            .NotEmpty().WithMessage("Bangla name is required.")
            .MaximumLength(200).WithMessage("Bangla name must not exceed 200 characters.");

        RuleFor(x => x.Designation)
            .IsInEnum().WithMessage("Invalid designation.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

        RuleFor(x => x.QualificationEn)
            .MaximumLength(500).WithMessage("English qualification must not exceed 500 characters.");

        RuleFor(x => x.QualificationBn)
            .MaximumLength(500).WithMessage("Bangla qualification must not exceed 500 characters.");

        RuleFor(x => x.SubjectEn)
            .MaximumLength(200).WithMessage("English subject must not exceed 200 characters.");

        RuleFor(x => x.SubjectBn)
            .MaximumLength(200).WithMessage("Bangla subject must not exceed 200 characters.");
    }
}
