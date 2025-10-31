using Backend.Domain;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(f => f.Id);
        builder.HasIndex(f => f.Name)
            .IsUnique();
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}