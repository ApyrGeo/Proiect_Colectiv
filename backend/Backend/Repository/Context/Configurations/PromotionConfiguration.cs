using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(gy => gy.Id);

        builder.Property(ssg => ssg.StartYear)
            .IsRequired();

        builder.Property(ssg => ssg.EndYear)
            .IsRequired();

		builder.HasOne(gy => gy.Specialisation)
            .WithMany(s => s.Promotions)
            .HasForeignKey(gy => gy.SpecialisationId);
    }
}
