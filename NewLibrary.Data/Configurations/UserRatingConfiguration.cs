using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLibrary.Core.Entities;

namespace NewLibrary.Data.Configurations
{
    public class UserRatingConfiguration:IEntityTypeConfiguration<UserRating>
    {
        public void Configure(EntityTypeBuilder<UserRating> builder)
        {
            builder.ToTable("user_ratings");

            builder.HasKey(ur => new { ur.AppUserId, ur.BookId });

            builder.HasOne(ur => ur.AppUser)
                .WithMany() 
                .HasForeignKey("AppUserId") 
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.BookEntity)
                .WithMany() 
                .HasForeignKey("BookId") 
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ur => ur.Rating)
                .IsRequired()
                .HasColumnName("rating");
        }
    }
}
