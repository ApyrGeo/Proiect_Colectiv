namespace Utils.Security;

public interface IAdapterPasswordHasher<TUser>
{
    string HashPassword(TUser user, string password);
}
