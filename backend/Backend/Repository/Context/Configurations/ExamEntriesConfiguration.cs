using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class ExamEntriesConfiguration : IEntityTypeConfiguration<ExamEntry>
{
    public void Configure(EntityTypeBuilder<ExamEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExamDate)
               .IsRequired(false) 
               .HasConversion(new ValueConverter<DateTime?, DateTime?>(
                   v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null,
                   v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null));

        builder.Property(e => e.Duration)
            .IsRequired(false); 
            
        builder.HasOne(e => e.Classroom)
               .WithMany(c => c.RegisteredExams)
               .HasForeignKey(e => e.ClassroomId)
               .IsRequired(false); 

        builder.HasOne(e => e.Subject)
               .WithMany(s => s.RegisteredExams)
               .HasForeignKey(e => e.SubjectId)
               .IsRequired();

        builder.HasOne(e => e.StudentGroup)
               .WithMany(sg => sg.RegisteredExams)
               .HasForeignKey(e => e.StudentGroupId)
               .IsRequired();
    }
}
