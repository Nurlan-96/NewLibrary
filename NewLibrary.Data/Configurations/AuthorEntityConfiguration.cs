using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLibrary.Core.Entities;

namespace NewLibrary.Data.Configurations
{
    public class AuthorEntityConfiguration : IEntityTypeConfiguration<AuthorEntity>
    {
        public void Configure(EntityTypeBuilder<AuthorEntity> builder)
        {
            builder.ToTable("author");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            builder.Property(a => a.Birthday)
                .HasColumnName("birthday")
                .HasColumnType("date");

            builder.Property(a => a.Image)
                .HasMaxLength(255)
                .HasColumnName("image");

            builder.Property(a => a.Bio)
                .HasMaxLength(5000)
                .HasColumnName("bio");

            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
