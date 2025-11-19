using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TrackForUBB.Domain.Security;

namespace TrackForUBB.Backend.Security;

public class AspNetCorePasswordHasher<TUser> : IAdapterPasswordHasher<TUser> where TUser : class
{
    private readonly PasswordHasher<TUser> _passwordHasher;

    public AspNetCorePasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _passwordHasher = new PasswordHasher<TUser>(options);
    }

    public string HashPassword(TUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }
}
