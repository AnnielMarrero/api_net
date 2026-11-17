using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Api.Domain.Entities;

namespace TaskManagement.Api.Infrastructure.Persistence;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(task => task.Description)
            .HasMaxLength(2000);
        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(task => task.Responsible)
            .HasMaxLength(150)
            .IsRequired();
        builder.Property(task => task.DueDateUtc)
            .IsRequired();
        builder.Property(task => task.CreatedAtUtc)
            .IsRequired();
        builder.Property(task => task.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(task => task.Status);
        builder.HasIndex(task => task.Responsible);
        builder.HasIndex(task => task.DueDateUtc);
    }
}
