using FluentValidation;

namespace TrackForUBB.Service.Interfaces;

public interface IValidatorFactory
{
    IValidator<T> Get<T>();
}
