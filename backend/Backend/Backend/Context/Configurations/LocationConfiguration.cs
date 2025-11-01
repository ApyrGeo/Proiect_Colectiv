using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);
        }
    }
}
