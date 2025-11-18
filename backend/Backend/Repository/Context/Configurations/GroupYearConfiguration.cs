using Domain;
using Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Context.Configurations;

public class GroupYearConfiguration : IEntityTypeConfiguration<GroupYear>
{
    public void Configure(EntityTypeBuilder<GroupYear> builder)
    {
        builder.HasKey(gy => gy.Id);

        builder.Property(ssg => ssg.Year)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.HasOne(gy => gy.Specialisation)
            .WithMany(s => s.GroupYears)
            .HasForeignKey(gy => gy.SpecialisationId);
    }
}
