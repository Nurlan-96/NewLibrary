using Microsoft.AspNetCore.Mvc;
using NewLibrary.Application.Commands.AuthorCommands;
using NewLibrary.Application.Commands.BookCommands;
using NewLibrary.Application.Queries.AuthorQueries;
using NewLibrary.Application.Repositories;
using NewLibrary.Application.Services.AuthorServices;
using NewLibrary.Application.Services.BookServices;

namespace NewLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(IAuthorRepository authorRepository, IAuthorQuery authorQuery, IAuthorService authorService) : ControllerBase
    {
        private readonly IAuthorRepository _authorRepo = authorRepository;
        private readonly IAuthorQuery _authorQuery = authorQuery;
        private readonly IAuthorService _authorService = authorService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, int size)
        {
            return Ok(await _authorQuery.GetAllAuthors(page, size));
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            return Ok(await _authorQuery.GetAuthorById(id));
        }
        
        [HttpGet("book")]
        public async Task<IActionResult> GetAuthorByBook([FromQuery] int id)
        {
            return Ok(await _authorQuery.GetAuthorByBookAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromForm] CreateAuthorCommand command)
        {
            return Ok(await _authorService.CreateAuthor(command));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAuthor([FromForm] int bookId)
        {
            return Ok(await _authorService.DeleteAuthor(bookId));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAuthor([FromForm] UpdateAuthorCommand command)
        {
            return Ok(await _authorService.UpdateAuthor(command));
        }
    }
}
