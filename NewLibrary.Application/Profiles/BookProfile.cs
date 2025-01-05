using AutoMapper;
using NewLibrary.Application.Response;
using NewLibrary.Core.Entities;

namespace NewLibrary.Application.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookEntity, BookResponse>();
            CreateMap<AuthorEntity, AuthorResponse>();
        }
    }
}
