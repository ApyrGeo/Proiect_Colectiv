using FluentValidation;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Service.Interfaces;

namespace TrackForUBB.Service.Validators;
public class UserPutDTOValidator : AbstractValidator<UserPutDTO>
{
    private readonly IUserRepository _repository;
    public UserPutDTOValidator(IUserRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User id is required.")
            .MustAsync(async (id, cancellation) =>
            {
                var existingUser = await _repository.GetByIdAsync(id);
                return existingUser != null;
            }).WithMessage("The specified user does not exist.");
    }
}