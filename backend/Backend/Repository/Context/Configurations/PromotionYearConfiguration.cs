using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class PromotionYearConfiguration : IEntityTypeConfiguration<PromotionYear>
{
    public void Configure(EntityTypeBuilder<PromotionYear> builder)
    {
        builder.HasKey(py => py.Id);

        builder.Property(py => py.YearNumber)
               .IsRequired();

        builder.HasOne(py => py.Promotion)
            .WithMany(p => p.Years)
            .HasForeignKey(py => py.PromotionId);
	}
}
