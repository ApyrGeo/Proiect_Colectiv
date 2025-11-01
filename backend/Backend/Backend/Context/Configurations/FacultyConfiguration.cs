using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasKey(f => f.Id);

        builder.HasIndex(f => f.Name)
            .IsUnique();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);
    }
}
