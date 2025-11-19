using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service.Validators;

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
