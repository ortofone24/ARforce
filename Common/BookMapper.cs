using ARforce.Models;

namespace ARforce.Common
{
    public interface IBookMapper
    {
        UpdateBook MapToUpdateBook(UpdateBookDto dto);
        Book MapToBookFromUpdateBook(Book existingBook, UpdateBook updateBookWithRowVersion);
        Book MapToBookFromCreateBook(CreateBookDto bookDto);
        ReturnBookDto MapToReturnBookDto(Book book);
    }

    public class BookMapper : IBookMapper
    {
        private readonly byte[] _rowVersion = { 0, 0, 0, 1 }; //This value is set only for simulation concurrency 
        public UpdateBook MapToUpdateBook(UpdateBookDto dto)
        {
            return new UpdateBook
            {
                Title = dto.Title,
                Author = dto.Author,
                Status = dto.Status,
                RowVersion = _rowVersion
            };
        }

        public Book MapToBookFromUpdateBook(Book existingBook, UpdateBook updateBookWithRowVersion)
        {
            existingBook.Title = updateBookWithRowVersion.Title;
            existingBook.Author = updateBookWithRowVersion.Author;
            existingBook.Status = updateBookWithRowVersion.Status;

            return existingBook;
        }

        public Book MapToBookFromCreateBook(CreateBookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Status = bookDto.Status,
                RowVersion = _rowVersion 
            };
            return book;
        }

        public ReturnBookDto MapToReturnBookDto(Book book)
        {
            return new ReturnBookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Status = book.Status
            };
        }
    }
}