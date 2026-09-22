using IdeaForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeaForge.Infrastructure.Persistence.Configurations;

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.ToTable("Ideas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Department)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.SubmittedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ValueScore)
            .IsRequired();

        builder.Property(x => x.FeasibilityScore)
            .IsRequired();

        builder.Property(x => x.UrgencyScore)
            .IsRequired();

        builder.Property(x => x.RiskScore)
            .IsRequired();

        builder.Property(x => x.PriorityScore)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.RejectionReason)
            .HasColumnType("text");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
