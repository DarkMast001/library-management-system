using LibraryManagementSystem.Entities;

namespace LibraryManagementSystem;

internal class BorrowingRecord
{
    public User User { get; set; }
    public Book Book { get; set; }
    public DateOnly Date { get; set; }

    public BorrowingRecord(User user, Book book, DateOnly date)
    {
        User = user;
        Book = book;
        Date = date;
    }
}
