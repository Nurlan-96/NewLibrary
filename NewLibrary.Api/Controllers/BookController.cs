using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLibrary.Application.Commands.BookCommands;
using NewLibrary.Application.Queries.BookQueries;
using NewLibrary.Application.Repositories;
using NewLibrary.Application.Services.BookServices;

namespace NewLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookRepository bookRepository, IBookQuery bookQuery, IBookService bookService) : ControllerBase
    {
        private readonly IBookRepository _bookRepo = bookRepository;
        private readonly IBookQuery _bookQuery = bookQuery;
        private readonly IBookService _bookService = bookService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, int size)
        {
            return Ok(await _bookQuery.GetAllBooks(page, size));
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            return Ok(await _bookQuery.GetBookById(id));
        }
        [HttpGet("genre")]
        public async Task<IActionResult> GetByGenre([FromQuery] int genreId, int page, int size)
        {
            return Ok(await _bookQuery.GetBooksByGenreAsync(genreId, page, size));
        }
        [HttpGet("author")]
        public async Task<IActionResult> GetBooksByAuthor([FromQuery] int authorId, int page, int size)
        {
            return Ok(await _bookQuery.GetBooksByAuthorAsync(authorId, page, size));
        }
        [HttpGet("popularity")]
        public async Task<IActionResult> GetBooksByPopularity([FromQuery] int page, int size)
        {
            return Ok(await _bookQuery.GetBooksByPopularityAsync(page, size));
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookCommand command)
        {
            return Ok(await _bookService.CreateBook(command));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBook([FromForm] int bookId)
        {
            return Ok(await _bookService.DeleteBook(bookId));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromForm] UpdateBookCommand command)
        {
            return Ok(await _bookService.UpdateBook(command));
        }
        [HttpPatch]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> RateBook([FromForm] int bookId, int rating)
        {
            return Ok(await _bookService.RateBook(bookId, rating));
        }
    }
}
