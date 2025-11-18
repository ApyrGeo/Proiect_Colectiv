using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using IValidatorFactory = Service.Interfaces.IValidatorFactory;

namespace Service.Validators;

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
