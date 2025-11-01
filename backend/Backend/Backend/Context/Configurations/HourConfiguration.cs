using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations
{
    public class HourConfiguration : IEntityTypeConfiguration<Hour>
    {
        public void Configure(EntityTypeBuilder<Hour> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Day)
                .IsRequired();

            builder.Property(x => x.Format)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            builder.Property(x => x.Frequency)
                .IsRequired();

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            builder.Property(x => x.HourInterval)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            builder.HasOne(x => x.Teacher)
                .WithMany(x => x.Hours)
                .HasForeignKey(x => x.TeacherId)
                .IsRequired();
        }
    }
}
