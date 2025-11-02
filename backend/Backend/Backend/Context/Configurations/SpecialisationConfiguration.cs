using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class SpecialisationConfiguration : IEntityTypeConfiguration<Specialisation>
{
    public void Configure(EntityTypeBuilder<Specialisation> builder)
    { 
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(s => s.Faculty)
            .WithMany(f => f.Specialisations)
            .HasForeignKey(s => s.FacultyId);

        builder.HasMany(s => s.GroupYears)
            .WithOne(gy => gy.Specialisation)
            .HasForeignKey(gy => gy.SpecialisationId);
    }
}
