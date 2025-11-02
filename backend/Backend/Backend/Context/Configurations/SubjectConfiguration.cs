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
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(f => f.NumberOfCredits)
            .IsRequired();
        
        builder.HasOne(f => f.GroupYear)
            .WithMany(f => f.Subjects)
            .HasForeignKey(sg => sg.GroupYearId);
    }
}