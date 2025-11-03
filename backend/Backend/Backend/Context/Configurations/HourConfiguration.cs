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

            builder.Property(x => x.Frequency)
                .IsRequired();

            builder.Property(x => x.HourInterval)
                .IsRequired()
                .HasMaxLength(Constants.DefaultStringMaxLenght);

            builder.HasOne(x => x.Subject)
                .WithMany(x => x.Hours)
                .HasForeignKey(x => x.SubjectId)
                .IsRequired();

            builder.HasOne(x => x.Teacher)
                .WithMany(x => x.Hours)
                .HasForeignKey(x => x.TeacherId)
                .IsRequired();

            builder.HasOne(x => x.Classroom)
                .WithMany(x => x.Hours)
                .HasForeignKey(x => x.ClassroomId)
                .IsRequired();

            builder.HasOne(x => x.GroupYear)
                .WithMany()
                .HasForeignKey(x => x.GroupYearId)
                .IsRequired(false);

            builder.HasOne(x => x.StudentGroup)
                .WithMany()
                .HasForeignKey(x => x.StudentGroupId)
                .IsRequired(false);

            builder.HasOne(x => x.StudentSubGroup)
                .WithMany()
                .HasForeignKey(x => x.StudentSubGroupId)
                .IsRequired(false);
        }
    }
}
