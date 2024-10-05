using System.ComponentModel.DataAnnotations;

namespace ARforce.Models
{
    public record UpdateBook
    {
        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        public BookStatus Status { get; set; } = BookStatus.OnShelf;

        [Required]
        public byte[] RowVersion { get; set; }
    }
}
