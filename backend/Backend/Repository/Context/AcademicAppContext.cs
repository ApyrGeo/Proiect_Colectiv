using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context;

public class AcademicAppContext(DbContextOptions<AcademicAppContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<StudentSubGroup> SubGroups { get; set; }
    public DbSet<StudentGroup> Groups { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<PromotionYear> PromotionYears { get; set; }
    public DbSet<PromotionSemester> PromotionSemesters { get; set; }
    public DbSet<Grade> Grades { get; set; }
	public DbSet<Specialisation> Specialisations { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Hour> Hours { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ExamEntry> ExamEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
