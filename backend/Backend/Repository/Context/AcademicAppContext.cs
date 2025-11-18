using Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Repository.Context;

public class AcademicAppContext(DbContextOptions<AcademicAppContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<StudentSubGroup> SubGroups { get; set; }
    public DbSet<StudentGroup> Groups { get; set; }
    public DbSet<GroupYear> GroupYears { get; set; }
    public DbSet<Specialisation> Specialisations { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Hour> Hours { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
