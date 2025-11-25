using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.Context.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Semester)
               .WithMany(s => s.Contracts)
               .HasForeignKey(x => x.SemesterId);

        builder.HasOne(x => x.Enrollment)
            .WithMany(s => s.Contracts)
            .HasForeignKey(x => x.EnrollmentId);
	}
}
