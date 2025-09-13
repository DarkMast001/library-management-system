using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Enums;
using System.Text;

namespace LibraryManagementSystem;

internal class Library : LibraryOperations
{
    private Dictionary<string, Book> _books;
    private Dictionary<string, User> _users;

    private HashSet<string> _genres;

    private List<BorrowingRecord> _borrowingHistory;
    
    public Library()
    {
        _books = new Dictionary<string, Book>();
        _users = new Dictionary<string, User>();
        _genres = new HashSet<string>();
        _borrowingHistory = new List<BorrowingRecord>();
    }

    public Library(string filePath) : this()
    {
        LoadDataFromFile(filePath);
    }

    public void AddBook(string title, string author, string isbn, string genre)
    {
        Book book = new Book(title, author, isbn, genre);

        _genres.Add(genre);

        try
        {
            _books.Add(isbn, book);
        }
        catch (ArgumentException)
        {
            throw;
        }
    }

    public bool RemoveBook(string isbn)
    {
        var book = FindBook(isbn);

        if (book != null && !book.IsAvailable)
        {
            return false;
        }

        bool flag = _books.Remove(isbn, out book);

        if (book is not null)
        {
            if (SearchBooks($"genre:{book.Genre}").Count == 0)
            {
                _genres.Remove(book.Genre);
            }
        }

        return flag;
    }

    public Book? FindBook(string isbn)
    {
        _books.TryGetValue(isbn, out var book);

        return book;
    }

    public IReadOnlyCollection<Book> SearchBooks(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<Book>();

        var results = _books.Values.AsEnumerable();

        var parts = query.Split(' ');

        foreach (var part in parts)
        {
            if (part.Contains(":"))
            {
                var filterField = part.Split(':', 2);
                string field = filterField[0].ToLower();
                string filter = filterField[1].ToLower();

                switch (field)
                {
                    case "title":
                        results = results.Where(b => b.Title.ToLower().Contains(filter) == true);
                        break;
                    case "author":
                        results = results.Where(b => b.Author.ToLower().Contains(filter) == true);
                        break;
                    case "isbn":
                        results = results.Where(b => b.Isbn.ToLower() == filter);
                        break;
                    case "genre":
                        results = results.Where(b => b.Genre.ToLower().Contains(filter) == true);
                        break;
                    default:
                        results = results.Where(b =>
                            b.Title?.ToLower().Contains(filter) == true ||
                            b.Author?.ToLower().Contains(filter) == true ||
                            b.Genre?.ToLower().Contains(filter) == true);
                        break;
                }
            }
        }

        return results.ToList();
    }

    public IReadOnlyCollection<Book> GetAllBooks()
    {
        return _books.Values.ToList();
    }

    public void RegisterUser(string name, string userId, string email, UserType type)
    {
        User user;

        if (type == UserType.STUDENT)
        {
            user = new Student(name, userId, email);
        }
        else if (type == UserType.FACULTY)
        {
            user = new Faculty(name, userId, email);
        }
        else if (type == UserType.GUEST)
        {
            user = new Guest(name, userId, email);
        }
        else
        {
            throw new TypeAccessException();
        }

        try
        {
            _users.Add(user.UserId, user);
        }
        catch (ArgumentException)
        {
            throw;
        }
    }

    public User? FindUser(string userId)
    {
        _users.TryGetValue(userId, out var user);

        return user;
    }

    public IReadOnlyCollection<User> GetAllUsers()
    {
        return _users.Values.ToList();
    }

    public bool BorrowBook(string userId, string isbn)
    {
        _users.TryGetValue(userId, out var user);
        _books.TryGetValue(isbn, out var book);

        if (user is null || book is null)
        {
            return false;
        }
        if (!user.CanBorrow())
        {
            return false;
        }
        if (!book.IsAvailable)
        {
            return false;
        }

        book.IsAvailable = false;
        book.BorrowTime = DateTime.Today;
        user.AddBorrowedBook(book);

        _borrowingHistory.Add(new BorrowingRecord(user, book, DateOnly.FromDateTime(DateTime.Now)));

        return true;
    }

    public bool ReturnBook(string userId, string isbn)
    {
        _users.TryGetValue(userId, out var user);
        _books.TryGetValue(isbn, out var book);

        if (user is null || book is null)
        {
            return false;
        }

        if (user.RemoveBorrowedBook(book))
        {
            book.IsAvailable = true;

            DateTime today = DateTime.Today;

            TimeSpan difference = today - book.BorrowTime;
            if (difference.Days > user.GetBorrowDays())
            {
                double fine = (difference.Days - user.GetBorrowDays()) * user.GetFinePerDay();
                user.SumUpFines(fine);
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    private void LoadDataFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не найден: {filePath}");
        }

        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || !trimmedLine.Contains(":"))
                continue;

            string[] parts = trimmedLine.Split(',');
            if (parts.Length < 4)
                continue;

            string entityType = parts[0].Split(':')[0].Trim();
            string idOrType = parts[0].Split(':')[1].Trim();

            try
            {
                switch (entityType.ToLower())
                {
                    case "user":
                    {
                        string userId = idOrType;
                        string name = parts[1].Trim();
                        string email = parts[2].Trim();
                        if (!int.TryParse(parts[3].Trim(), out int typeValue))
                            throw new FormatException($"Некорректный тип пользователя: {parts[3]}");

                        UserType userType = typeValue switch
                        {
                            1 => UserType.STUDENT,
                            2 => UserType.FACULTY,
                            3 => UserType.GUEST,
                            _ => throw new ArgumentException($"Неизвестный тип пользователя: {typeValue}")
                        };

                        RegisterUser(name, userId, email, userType);
                        break;
                    }

                    case "book":
                    {
                        string isbn = idOrType;
                        string title = parts[1].Trim();
                        string author = parts[2].Trim();
                        string genre = parts[3].Trim();

                        AddBook(title, author, isbn, genre);
                        break;
                    }

                    default:
                        Console.WriteLine($"Неизвестный тип сущности: {entityType}");
                        break;
                }
            }
            catch (Exception ex) when (!(ex is FileNotFoundException || ex is DirectoryNotFoundException))
            {
                Console.WriteLine($"Ошибка при обработке строки: '{line}'\n{ex.Message}");
            }
        }
    }
}
