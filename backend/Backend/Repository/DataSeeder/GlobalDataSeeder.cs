namespace TrackForUBB.Repository.DataSeeder;

public class GlobalDataSeeder(UniversityDataSeeder universityDataSeeder, UserDataSeeder userDataSeeder, LocationDataSeeder locationDataSeeder, TeacherDataSeeder teacherDataSeeder, SubjectDataSeeder subjectDataSeeder, HourDataSeeder hourDataSeeder)
{
    private readonly UniversityDataSeeder _universityDataSeeder = universityDataSeeder;
    private readonly UserDataSeeder _userDataSeeder = userDataSeeder;
    private readonly LocationDataSeeder _locationDataSeeder = locationDataSeeder;
    private readonly TeacherDataSeeder _teacherDataSeeder = teacherDataSeeder;
    private readonly SubjectDataSeeder _subjectDataSeeder = subjectDataSeeder;
    private readonly HourDataSeeder _hourDataSeeder = hourDataSeeder;

    public async Task SeedAsync()
    {
        await _universityDataSeeder.SeedAsync();
        await _userDataSeeder.SeedAsync();
        await _locationDataSeeder.SeedAsync();
        await _teacherDataSeeder.SeedAsync();
        await _subjectDataSeeder.SeedAsync();
        await _hourDataSeeder.SeedAsync();
    }
}
