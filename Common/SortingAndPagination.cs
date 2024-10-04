using ARforce.Models;
using Microsoft.EntityFrameworkCore;

namespace ARforce.Common
{
    public static class SortingAndPagination
    {
        public static async Task<List<Book>> Items(int page, int pageSize, IQueryable<Book> query)
        {
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return items;
        }

        public static IQueryable<Book> SortingBy(string sortBy, IQueryable<Book> query)
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
