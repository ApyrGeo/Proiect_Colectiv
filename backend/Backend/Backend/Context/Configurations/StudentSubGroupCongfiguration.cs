using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class StudentSubGroupCongfiguration : IEntityTypeConfiguration<StudentSubGroup>
{
    public void Configure(EntityTypeBuilder<StudentSubGroup> builder)
    {
        builder.HasKey(ssg => ssg.Id);

        builder.HasIndex(sg => sg.Name)
            .IsUnique();
        builder.Property(sg => sg.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(sg => sg.Enrollments)
            .WithOne(e => e.SubGroup)
            .HasForeignKey(e => e.SubGroupId);

        builder.HasOne(sg => sg.StudentGroup)
            .WithMany(g => g.StudentSubGroups)
            .HasForeignKey(sg => sg.StudentGroupId);
    }
}
