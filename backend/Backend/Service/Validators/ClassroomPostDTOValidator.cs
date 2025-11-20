using TrackForUBB.Domain.DTOs;
using FluentValidation;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;

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
