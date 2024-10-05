using ARforce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ARforce.Common
{
    public interface ISortingAndPagination
    {
        Task<List<ReturnBookDto>> Items(int page, int pageSize, IQueryable<Book> query);
        IQueryable<Book> SortingBy(string sortBy, IQueryable<Book> query);
    }

    public class SortingAndPagination : ISortingAndPagination
    {
        public async Task<List<ReturnBookDto>> Items(int page, int pageSize, IQueryable<Book> query)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            var itemsQuery = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(book => new ReturnBookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Status = book.Status
                });

            if (query.Provider is IAsyncQueryProvider)
            {
                return await itemsQuery.ToListAsync();
            }
            else
            {
                return itemsQuery.ToList();
            }
        }

        public IQueryable<Book> SortingBy(string sortBy, IQueryable<Book> query)
        {
            query = sortBy?.ToLower() switch
            {
                "title" => query.OrderBy(b => b.Title),
                "author" => query.OrderBy(b => b.Author),
                "isbn" => query.OrderBy(b => b.ISBN),
                "status" => query.OrderBy(b => b.Status),
                _ => query.OrderBy(b => b.Id),
            };
            return query;
        }
    }
}
