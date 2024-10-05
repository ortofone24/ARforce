namespace ARforce.Models
{
    public record GetBooksResponse
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<ReturnBookDto> Items { get; set; }
    }
}
