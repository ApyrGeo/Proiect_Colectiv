namespace Backend.Interfaces;

public interface IPasswordHashHelper
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
