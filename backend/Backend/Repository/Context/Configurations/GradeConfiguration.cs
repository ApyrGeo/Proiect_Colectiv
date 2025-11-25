using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
	public void Configure(EntityTypeBuilder<Grade> builder)
	{
		builder.HasKey(g => g.Id);

		builder.Property(g => g.Value)
			.IsRequired();

		builder.HasOne(g => g.Enrollment)
			.WithMany(e => e.Grades)
			.HasForeignKey(g => g.EnrollmentId);

		builder.HasOne(g => g.Subject)
			.WithMany(s => s.Grades)
			.HasForeignKey(g => g.SubjectId);

		builder.HasOne(g => g.Semester)
			.WithMany(s => s.Grades)
			.HasForeignKey(g => g.SemesterId);
	}
}