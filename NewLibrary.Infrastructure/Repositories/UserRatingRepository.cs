using Microsoft.EntityFrameworkCore;
using NewLibrary.Application.Repositories;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;

namespace NewLibrary.Infrastructure.Repositories
{
    public class UserRatingRepository(AppDbContext context) : Repository<UserRating>, IUserRatingRepository
    {
        public sealed override DbContext Context { get; protected set; } = context;
    }
}
