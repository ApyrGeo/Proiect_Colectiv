using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.HasKey(sg => sg.Id);
        
        builder.Property(sg => sg.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.HasOne(sg => sg.GroupYear)
            .WithMany(gy => gy.StudentGroups)
            .HasForeignKey(sg => sg.GroupYearId);
    }
}
