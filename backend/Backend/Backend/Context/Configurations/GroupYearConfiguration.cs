using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class GroupYearConfiguration : IEntityTypeConfiguration<GroupYear>
{
    public void Configure(EntityTypeBuilder<GroupYear> builder)
    {
        builder.HasKey(gy => gy.Id);

        builder.HasIndex(gy => gy.Year)
            .IsUnique();
        builder.Property(ssg => ssg.Year)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(gy => gy.StudentGroups)
            .WithOne(sg => sg.GroupYear)
            .HasForeignKey(sg => sg.GroupYearId);

        builder.HasOne(gy => gy.Specialisation)
            .WithMany(s => s.GroupYears)
            .HasForeignKey(gy => gy.SpecialisationId);
        
        builder.HasMany(gy => gy.Subjects)
            .WithOne(sg => sg.GroupYear)
            .HasForeignKey(sg => sg.GroupYearId);
    }
}
