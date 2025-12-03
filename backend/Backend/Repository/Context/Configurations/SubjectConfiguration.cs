using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.Property(f => f.NumberOfCredits)
            .IsRequired();

        builder.HasMany(s => s.Contracts)
            .WithMany(c => c.Subjects);

        builder.HasOne(s => s.HolderTeacher)
            .WithOne(t => t.HeldSubject)
            .HasForeignKey<Teacher>(t => t.HeldSubjectId)
            .IsRequired(false);
    }
}