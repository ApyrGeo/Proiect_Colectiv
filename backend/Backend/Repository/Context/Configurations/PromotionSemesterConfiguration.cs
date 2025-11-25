using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class PromotionSemesterConfiguration : IEntityTypeConfiguration<PromotionSemester>
{
	public void Configure(EntityTypeBuilder<PromotionSemester> builder)
	{
		builder.HasKey(ps => ps.Id);

		builder.Property(ps => ps.SemesterNumber)
			   .IsRequired();

		builder.HasOne(ps => ps.PromotionYear)
			.WithMany(py => py.PromotionSemesters)
			.HasForeignKey(ps => ps.PromotionYearId);
	}
}
