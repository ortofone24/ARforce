using ARforce.Common;
using ARforce.Infrastructure;
using ARforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ARforce.Controllers
{
    public class BooksController(LibraryContext _context) : ApiController
    {
        // GET: api/Books
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string sortBy = "Id", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Books.AsQueryable();

            // SortingBy
            query = SortingAndPagination.SortingBy(sortBy, query);

            // Pagination
            var totalItems = await query.CountAsync();
            var items = await SortingAndPagination.Items(page, pageSize, query);

            //var result = new
            //{
            //    TotalItems = totalItems,
            //    Page = page,
            //    PageSize = pageSize,
            //    Items = items
            //};
            var result = new GetBooksResponse
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };

            return Ok(result);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/Books
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Books.Any(b => b.ISBN == bookDto.ISBN))
            {
                ModelState.AddModelError("ISBN", "ISBN must be unique.");
                return BadRequest(ModelState);
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Status = bookDto.Status
            };

            _context.Books.Add(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while saving data. Please try again later." });
            }

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updatedBook)
        {
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
                return NotFound();

            if (!BookStatusValidator.IsValidTransition(existingBook.Status, updatedBook.Status))
            {
                return BadRequest($"Invalid status change from {existingBook.Status} to {updatedBook.Status}.");
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.Status = updatedBook.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while saving data. Please try again later." });
            }

            return NoContent();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
