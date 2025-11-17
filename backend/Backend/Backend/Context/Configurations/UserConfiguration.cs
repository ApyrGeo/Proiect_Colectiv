using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

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

        builder.Property(user => user.Password)
            .IsRequired()
            .HasMaxLength(Constants.ExtendedStringMaxLenght);

        builder.Property(user => user.Role)
            .IsRequired();
    }
}
