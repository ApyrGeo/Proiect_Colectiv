using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(builder => builder.Id);

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        //TODO: Enable TenantEmail uniqueness after deleting test data from db

        //builder.Property(user => user.TenantEmail)
        //    .IsRequired()
        //    .HasMaxLength(Constants.DefaultStringMaxLenght);

        //builder.HasIndex(user => user.TenantEmail)
        //    .IsUnique();

        builder.Property(user => user.TenantEmail)
            .IsRequired(false)
            .HasMaxLength(Constants.ExtendedStringMaxLenght);

        builder.Property(user => user.Owner)
            .IsRequired(false);

        builder.Property(user => user.Role)
            .IsRequired();
        
        builder.Property(user => user.Signature)
            .IsRequired(false);
    }
}
