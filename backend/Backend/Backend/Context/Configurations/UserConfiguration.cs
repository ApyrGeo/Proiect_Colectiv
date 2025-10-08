using Backend.Domain;
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
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.Password)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(user => user.Role)
            .IsRequired();

        builder.HasMany(user => user.Enrollments)
            .WithOne(enrollment => enrollment.User)
            .HasForeignKey(enrollment => enrollment.UserId);
    }
}
