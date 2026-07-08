using FluentValidation;
using School.Application.DTOs.Contact;

namespace School.Application.Validators;

public class ContactCreateDtoValidator : AbstractValidator<ContactCreateDto>
{
    public ContactCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.");
    }
}
