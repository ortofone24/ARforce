using System.ComponentModel.DataAnnotations;

namespace ARforce.Models
{
    public record UpdateBookDto
    {
        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        public BookStatus Status { get; set; } = BookStatus.OnShelf;
    }
}
