using Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasOne(x => x.SubGroup)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.SubGroupId)
                .IsRequired();
        }
    }
}
