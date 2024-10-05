using ARforce.Models;

namespace ARforce.Common
{
    public interface IBookStatusValidator
    {
        bool IsValidTransition(BookStatus currentStatus, BookStatus newStatus);
    }

    public class BookStatusValidator : IBookStatusValidator
    {
        public bool IsValidTransition(BookStatus currentStatus, BookStatus newStatus)
        {
            return newStatus switch
            {
                BookStatus.OnShelf => currentStatus == BookStatus.Returned || currentStatus == BookStatus.Damaged,
                BookStatus.Borrowed => currentStatus == BookStatus.OnShelf,
                BookStatus.Returned => currentStatus == BookStatus.Borrowed,
                BookStatus.Damaged => currentStatus == BookStatus.OnShelf || currentStatus == BookStatus.Returned,
                _ => false,
            };
        }
    }
}
