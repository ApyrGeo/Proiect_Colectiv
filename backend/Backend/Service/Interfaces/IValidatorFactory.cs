using FluentValidation;

namespace Service.Interfaces;

public interface IValidatorFactory
{
    IValidator<T> Get<T>();
}
