using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLibrary.Core.Constants;
using NewLibrary.Core.Entities;

namespace NewLibrary.Data.Configurations
{
    public class BookEntityConfiguration : IEntityTypeConfiguration<BookEntity>
    {
        public void Configure(EntityTypeBuilder<BookEntity> builder)
        {
            builder.ToTable("book");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            builder.Property(c => c.Pages).HasColumnName("pages");

            builder.Property(c => c.AuthorId).HasColumnName("author_id");

            builder.Property(c => c.YearPublished).HasColumnName("year_published");

            builder.Property(c => c.Rating)
                .HasColumnType("decimal(3,2)")
                .HasColumnName("rating");

            builder.Property(c => c.Description)
                .HasMaxLength(5000)
                .HasColumnName("description");

            builder.Property(c => c.Genre)
                .HasConversion(
                    g => string.Join(",", g.Select(e => e.ToString())),
                    g => g.Split(",", StringSplitOptions.RemoveEmptyEntries)
                          .Select(e => Enum.Parse<GenreEnum>(e))
                          .ToList()
                )
                .HasColumnName("genre");

            builder.Property(c => c.Image)
                .HasMaxLength(255)
                .HasColumnName("image");            
            
            builder.Property(c => c.Content)
                .HasMaxLength(255)
                .HasColumnName("content");

            builder.HasOne(c => c.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
