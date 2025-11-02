using Backend.Domain;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Context.Configurations;

public class StudentSubGroupCongfiguration : IEntityTypeConfiguration<StudentSubGroup>
{
    public void Configure(EntityTypeBuilder<StudentSubGroup> builder)
    {
        builder.HasKey(sg => sg.Id);

        builder.Property(sg => sg.Name)
            .IsRequired()
            .HasMaxLength(Constants.DefaultStringMaxLenght);

        builder.HasOne(sg => sg.StudentGroup)
            .WithMany(g => g.StudentSubGroups)
            .HasForeignKey(sg => sg.StudentGroupId);
    }
}
