using Backend.Domain.DTOs;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators;

public class LocationPostDTOValidator : AbstractValidator<LocationPostDTO>
{
    public LocationPostDTOValidator()
    {
        RuleFor(l => l.Name)
            .NotNull()
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Name cannot exceed {Constants.DefaultStringMaxLenght} characters.");

        RuleFor(l => l.Address)
            .NotNull()
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Address cannot exceed {Constants.DefaultStringMaxLenght} characters.");
    }
}
