using TrackForUBB.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Repository.Context.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(l => l.GoogleMapsData, gm =>
        {
            gm.Property(g => g.Id)
                .HasColumnName("GoogleMapsData_Id")
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            gm.Property(g => g.Latitude)
                .HasColumnName("GoogleMapsData_Latitude");

            gm.Property(g => g.Longitude)
                .HasColumnName("GoogleMapsData_Longitude");
        });

        builder.Navigation(l => l.GoogleMapsData)
            .IsRequired(false);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);
    }
}
