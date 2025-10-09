using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.HasKey(sg => sg.Id);
        
        builder.HasIndex(sg => sg.Name)
            .IsUnique();
        builder.Property(sg => sg.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(sg => sg.StudentSubGroups)
            .WithOne(s => s.StudentGroup)
            .HasForeignKey(s => s.StudentGroupId);

        builder.HasOne(sg => sg.GroupYear)
            .WithMany(gy => gy.StudentGroups)
            .HasForeignKey(sg => sg.GroupYearId);
    }
}
