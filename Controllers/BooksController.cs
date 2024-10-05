using ARforce.Common;
using ARforce.Infrastructure;
using ARforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ARforce.Controllers
{
    public class BooksController : ApiController
    {
        private readonly LibraryContext _context;
        private readonly IBookMapper _bookMapper;
        private readonly IBookStatusValidator _bookStatusValidator;
        private readonly ISortingAndPagination _sortingAndPagination;

        public BooksController(LibraryContext context, 
            IBookMapper bookMapper, 
            IBookStatusValidator bookStatusValidator, 
            ISortingAndPagination sortingAndPagination)
        {
            _context = context;
            _bookMapper = bookMapper;
            _bookStatusValidator = bookStatusValidator;
            _sortingAndPagination = sortingAndPagination;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string sortBy = "Id", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Books.AsQueryable();

            // SortingBy
            query = _sortingAndPagination.SortingBy(sortBy, query);

            // Pagination
            var totalItems = await query.CountAsync();
            var items = await _sortingAndPagination.Items(page, pageSize, query);

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

            var returnBook = _bookMapper.MapToReturnBookDto(book);

            return Ok(returnBook);
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

            var book = _bookMapper.MapToBookFromCreateBook(bookDto);

            _context.Books.Add(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while saving data. Please try again later." });
            }

            var returnBookDto = _bookMapper.MapToReturnBookDto(book);

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, returnBookDto);
        }



        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updatedBook)
        {
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
                return NotFound();

            if (!_bookStatusValidator.IsValidTransition(existingBook.Status, updatedBook.Status))
            {
                return BadRequest($"Invalid status change from {existingBook.Status} to {updatedBook.Status}.");
            }

            var updateBookWithRowVersion = _bookMapper.MapToUpdateBook(updatedBook);

            existingBook = _bookMapper.MapToBookFromUpdateBook(existingBook, updateBookWithRowVersion);

            //concurrency checking
            _context.Entry(existingBook).Property(b => b.RowVersion).OriginalValue = updateBookWithRowVersion.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();

                var databaseEntry = await entry.GetDatabaseValuesAsync();

                if (databaseEntry == null)
                {
                    return NotFound(new
                    {
                        message = "The record no longer exists. It may have been deleted by another user."
                    });
                }

                var databaseValues = (Book)databaseEntry.ToObject();

                return Conflict(new
                {
                    message = "The record you attempted to edit was modified by another user after you got the original value. Your edit operation was canceled.",
                    currentData = databaseValues
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating data. Please try again later."
                });
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting data. Please try again later." });
            }

            return NoContent();
        }
    }
}
