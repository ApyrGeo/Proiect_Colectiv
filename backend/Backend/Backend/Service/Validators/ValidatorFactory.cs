using FluentValidation;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service.Validators;

public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _provider;
    public ValidatorFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    public IValidator<T> Get<T>()
    {
        return _provider.GetRequiredService<IValidator<T>>();
    }
}
