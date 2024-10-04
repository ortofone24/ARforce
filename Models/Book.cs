using System.ComponentModel.DataAnnotations;

namespace ARforce.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; } 

        public BookStatus Status { get; set; } = BookStatus.OnShelf;
    }

}
