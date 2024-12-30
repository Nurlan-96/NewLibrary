using Microsoft.EntityFrameworkCore;
using NewLibrary.Application.Repositories;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;

namespace NewLibrary.Infrastructure.Repositories
{
    public class AuthorRepository(AppDbContext context) : Repository<AuthorEntity>, IAuthorRepository
    {
        public sealed override DbContext Context { get; protected set; } = context;
    }
}
