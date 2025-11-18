using Domain;
using Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Context.Configurations;

public class SpecialisationConfiguration : IEntityTypeConfiguration<Specialisation>
{
    public void Configure(EntityTypeBuilder<Specialisation> builder)
    { 
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.HasOne(s => s.Faculty)
            .WithMany(f => f.Specialisations)
            .HasForeignKey(s => s.FacultyId);
    }
}
