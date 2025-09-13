using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Enums;

namespace LibraryManagementSystem;

internal interface LibraryOperations
{
    void AddBook(string title, string author, string isbn, string genre);
    bool RemoveBook(string isbn);
    Book? FindBook(string isbn);
    IReadOnlyCollection<Book> SearchBooks(string query);
    IReadOnlyCollection<Book> GetAllBooks();

    void RegisterUser(string name, string userId, string email, UserType type);
    User? FindUser(string userId);
    IReadOnlyCollection<User> GetAllUsers();

    bool BorrowBook(string userId, string isbn);
    bool ReturnBook(string userId, string isbn);
}
