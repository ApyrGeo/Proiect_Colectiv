using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class StudentSubGroupCongfiguration : IEntityTypeConfiguration<StudentSubGroup>
{
    public void Configure(EntityTypeBuilder<StudentSubGroup> builder)
    {
        builder.HasKey(ssg => ssg.Id);

        builder.HasIndex(ssg => ssg.Name)
            .IsUnique();
        builder.Property(ssg => ssg.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(ssg => ssg.Enrollments)
            .WithOne(e => e.SubGroup)
            .HasForeignKey(e => e.SubGroupId);

        builder.HasOne(ssg => ssg.StudentGroup)
            .WithMany(sg => sg.StudentSubGroups)
            .HasForeignKey(ssg => ssg.StudentGroupId);
    }
}
