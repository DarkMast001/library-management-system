using LibraryManagementSystem.Entities;
using System.Text;

namespace LibraryManagementSystem;

internal abstract class User
{
    protected List<Book> borrowedBooks;

    protected double accumulatedFine;

    public User(string name, string userId, string email)
    {
        accumulatedFine = 0;
        Name = name;
        UserId = userId;
        Email = email;
        borrowedBooks = new List<Book>();
    }

    public abstract string Name { get; set; }
    public abstract string UserId { get; set; }
    public abstract string Email { get; set; }

    public abstract int GetMaxBooks();
    public abstract int GetBorrowDays();
    public abstract double GetFinePerDay();

    public void SumUpFines(double fine)
    {
        if (fine < 0)
        {
            return;
        }
        accumulatedFine += fine;
    }

    public void ResetTheFines()
    {
        accumulatedFine = 0;
    }

    public double GetAccumulatedFine()
    {
        return accumulatedFine;
    }

    public bool CanBorrow()
    {
        return borrowedBooks.Count < GetMaxBooks();
    }

    public void AddBorrowedBook(Book book)
    {
        if (borrowedBooks.Count < GetMaxBooks())
        {
            borrowedBooks.Add(book);
        }
        else
        {
            throw new InvalidOperationException("Превышен лимит выданных книг.");
        }
    }

    public bool RemoveBorrowedBook(Book book)
    {
        return borrowedBooks.Remove(book);
    }

    public IReadOnlyList<Book> GetBorrowedBooks()
    {
        return borrowedBooks.AsReadOnly();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"{UserId} - {Name} - {Email} - {accumulatedFine}");

        if (borrowedBooks.Count == 0)
        {
            sb.Append(" - Книжек не брал.");
        }
        else
        {
            sb.Append($" - взял книг: {borrowedBooks.Count}\n");
            for (int i = 0; i < borrowedBooks.Count; i++)
            {
                if (i == borrowedBooks.Count - 1)
                {
                    sb.Append($"\t{borrowedBooks[i].ToString()}");
                }
                else
                {
                    sb.Append($"\t{borrowedBooks[i].ToString()}\n");
                }
            }
        }

        return sb.ToString();
    }
}
