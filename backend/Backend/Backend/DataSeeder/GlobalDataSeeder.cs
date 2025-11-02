using Bogus;

namespace Backend.DataSeeder;

public class GlobalDataSeeder
{
    private readonly UniversityDataSeeder _universityDataSeeder;
    private readonly UserDataSeeder _userDataSeeder;

    public GlobalDataSeeder(UniversityDataSeeder universityDataSeeder, UserDataSeeder userDataSeeder)
    {
        _universityDataSeeder = universityDataSeeder;
        _userDataSeeder = userDataSeeder;
    }

    public async Task SeedAsync()
    {
        await _universityDataSeeder.SeedAsync();
        await _userDataSeeder.SeedAsync();
    }
}
