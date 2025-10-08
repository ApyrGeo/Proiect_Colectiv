using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasMany(f => f.Specialisations)
            .WithOne(s => s.Faculty)
            .HasForeignKey(s => s.FacultyId);

        builder.HasKey(f => f.Id);

        builder.HasIndex(f => f.Name)
            .IsUnique();
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
