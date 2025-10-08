using FluentValidation;

namespace Backend.Interfaces;

public interface IValidatorFactory
{
    IValidator<T> Get<T>();
}
