using Backend.Domain.DTOs;
using Backend.Interfaces;
using Backend.Repository;
using Backend.Utils;
using FluentValidation;

namespace Backend.Service.Validators
{
    public class ClassroomPostDTOValidator : AbstractValidator<ClassroomPostDTO>
    {
        public ClassroomPostDTOValidator(ITimetableRepository repo)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(Constants.DefaultStringMaxLenght).WithMessage($"Name cannot exceed {Constants.DefaultStringMaxLenght} characters.");

            RuleFor(x => x.LocationId)
                .MustAsync(async (locationId, cancellation) =>
                {
                    var location = await repo.GetLocationByIdAsync(locationId);
                    return location != null;
                }).WithMessage("The specified Location does not exist.");
        }
    }
}
