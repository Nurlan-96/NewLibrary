﻿using NewLibrary.Application.Response;
using NewLibrary.Core.Entities;

namespace NewLibrary.Application.Queries.BookQueries
{
    public interface IBookQuery
    {
        Task<Pagination<BookEntity>> GetAllBooks(int page, int size);
        Task<BookEntity> GetBookById(int id);
        Task<Pagination<BookResponse>> GetBooksByGenreAsync(int genreId, int skip, int take);
        Task<Pagination<BookResponse>> GetBooksByAuthorAsync(int id, int page, int size);
        Task<List<BookEntity>> GetBooksByPopularityAsync(int page, int size);
    }
}
