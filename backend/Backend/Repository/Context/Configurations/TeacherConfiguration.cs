using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Teacher>(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Faculty)
            .WithMany(x => x.Teachers)
            .HasForeignKey(x => x.FacultyId)
            .IsRequired();
    }
}
