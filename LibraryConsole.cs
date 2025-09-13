using LibraryManagementSystem.Entities;
using LibraryManagementSystem.Enums;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace LibraryManagementSystem;

internal class LibraryConsole
{
    private Library _library;

    public LibraryConsole(Library library)
    {
        if (_library is null)
        {
            _library = library;
        }
    }

    public void Run()
    {
        while(true)
        {
            ShowMainMenu();
            int choice = GetIntInput("Выберите действие: ");

            switch (choice)
            {
                case 1: HandleBookManagement(); break;
                case 2: HandleUserManagement(); break;
                case 3: HandleBorrowing(); break;
                case 0: return;
                default: Console.WriteLine("Неверный выбор"); break;
            }
        }
    }

    private void HandleBookManagement()
    {
        string title;
        string author;
        string isbn;
        string genre;

        while (true)
        {
            ShowBookManageMenu();
            int choice = GetIntInput("Выберите действие: ");

            switch (choice)
            {
                case 1:
                    title = GetStringInput("Введите заглавие: ");
                    author = GetStringInput("Введите автора: ");
                    isbn = GetStringInput("Введите isbn: ");
                    genre = GetStringInput("ВВедите жанр: ");

                    try
                    {
                        _library.AddBook(title, author, isbn, genre);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Не удалось добавить книгу. Книга с таким isbn уже существует.");
                    }

                    break;
                case 2:
                    isbn = GetStringInput("Введите isbn: ");

                    if (_library.RemoveBook(isbn))
                    {
                        Console.WriteLine("Книга успешно удалена.");
                    }
                    else 
                    {
                        Console.WriteLine("Не удалось удалить.");
                    }

                    break;
                case 3:
                    isbn = GetStringInput("Введите isbn: ");

                    Book? book = _library.FindBook(isbn);

                    if (book is null)
                    {
                        Console.WriteLine($"Не удалось найти книгу с isbn '{isbn}'.");
                    }
                    else
                    {
                        Console.WriteLine(book.ToString());
                    }

                    break;
                case 4:
                    Console.WriteLine("Введите только те пункты, по которым будет осуществлён поиск. " +
                        "Если искать по параметру не надо, то нажмите 'Enter'.");
                    title = GetStringInputAllowEneter("Введите заглавие: ");
                    author = GetStringInputAllowEneter("Введите автора: ");
                    isbn = GetStringInputAllowEneter("Введите isbn: ");
                    genre = GetStringInputAllowEneter("Введите жанр: ");

                    string query = FormQueryFromParams(title, author, isbn, genre);

                    var booksByQuery = _library.SearchBooks(query);

                    foreach (var b in booksByQuery)
                    {
                        Console.WriteLine(b.ToString());
                    }

                    break;
                case 5:
                    var books = _library.GetAllBooks();

                    foreach (var b in books)
                    {
                        Console.WriteLine(b.ToString());
                    }

                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }

    private void HandleUserManagement()
    {
        string name;
        string userId;
        string email;
        UserType type;

        while (true)
        {
            ShowUserManageMenu();
            int choice = GetIntInput("Выберите действие: ");

            switch (choice)
            {
                case 1:
                    name = GetStringInput("Введите имя: ");
                    userId = GetStringInput("Введите id: ");
                    email = GetStringInput("Введите почту: ");
                    type = GetUserTypeInput();

                    try
                    {
                        _library.RegisterUser(name, userId, email, type);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Не удалось добавить пользователя. Пользователь с таким id уже существует.");
                    }

                    break;
                case 2:
                    userId = GetStringInput("Введите id: ");

                    User? user = _library.FindUser(userId); 

                    if (user is null)
                    {
                        Console.WriteLine($"Не удалось найти пользователя с id '{userId}'.");
                    }
                    else
                    {
                        Console.WriteLine(user.ToString());
                    }

                    break;
                case 3: 
                    var users = _library.GetAllUsers(); 
                    
                    foreach(var u in users)
                    {
                        Console.WriteLine(u.ToString());
                    }

                    break;
                case 4:
                    userId = GetStringInput("Введите id: ");

                    user = _library.FindUser(userId);

                    if (user is null)
                    {
                        Console.WriteLine($"Не удалось найти пользователя с id '{userId}'.");
                    }
                    else
                    {
                        user.ResetTheFines();
                    }

                    break;
                case 0: 
                    return;
                default: 
                    Console.WriteLine("Неверный выбор."); 
                    break;
            }
        }
    }

    private void HandleBorrowing()
    {
        string userId;
        string isbn;

        while (true)
        {
            ShowIssueOperationsMenu();
            int choice = GetIntInput("Выберите действие: ");

            switch (choice)
            {
                case 1:
                    userId = GetStringInput("Введите id пользователя: ");
                    isbn = GetStringInput("Введите isbn книги: ");

                    if (_library.BorrowBook(userId, isbn)) 
                    {
                        Console.WriteLine("Книга успешно выдана.");
                    }
                    else 
                    { 
                        Console.WriteLine("Ошибка выдачи книги"); 
                    }

                    break;
                case 2:
                    userId = GetStringInput("Введите id пользователя: ");
                    isbn = GetStringInput("Введите isbn книги: ");

                    if (_library.ReturnBook(userId, isbn))
                    {
                        Console.WriteLine("Книга успешно возвращена.");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка возврата книги.");
                    }

                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }

    private void ShowIssueOperationsMenu()
    {
        Console.WriteLine("\n === Операции выдачи === ");
        Console.WriteLine("1. Выдать книгу");
        Console.WriteLine("2. Принять книгу");
        Console.WriteLine("0. Назад");
    }

    private void ShowBookManageMenu()
    {
        Console.WriteLine("\n === Управление книгами === ");
        Console.WriteLine("1. Добавить книгу");
        Console.WriteLine("2. Убрать книгу");
        Console.WriteLine("3. Найти книгу");
        Console.WriteLine("4. Найти книги по параметрам");
        Console.WriteLine("5. Вывести все книги");
        Console.WriteLine("0. Назад");
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("\n === Управление библиотекой === ");
        Console.WriteLine("1. Управление книгами");
        Console.WriteLine("2. Управление пользователями");
        Console.WriteLine("3. Операции выдачи");
        Console.WriteLine("0. Выход");
    }

    private void ShowUserManageMenu()
    {
        Console.WriteLine("\n === Управление пользователями === ");
        Console.WriteLine("1. Зарегистрировать пользователя");
        Console.WriteLine("2. Найти пользователя");
        Console.WriteLine("3. Вывести всех пользователей");
        Console.WriteLine("4. Списать долг с пользователя");
        Console.WriteLine("0. Назад");
    }

    private void ShowUserTypes()
    {
        Console.WriteLine("Выберите тип создаваемого пользователя");
        Console.WriteLine("1. Студент");
        Console.WriteLine("2. Преподаватель");
        Console.WriteLine("3. Гость");
    }

    private string FormQueryFromParams(string title, string author, string isbn, string genre)
    {
        StringBuilder sb = new StringBuilder();

        if (!title.Equals(""))
        {
            sb.Append($"title:{title} ");
        }

        if (!author.Equals(""))
        {
            sb.Append($"author:{author} ");
        }

        if (!isbn.Equals(""))
        {
            sb.Append($"isbn:{isbn} ");
        }

        if (!genre.Equals(""))
        {
            sb.Append($"genre:{genre} ");
        }

        return sb.ToString().Trim();
    }

    private int GetIntInput(string? promt = null)
    {
        while (true)
        {
            try
            {
                if (promt is not null)
                {
                    Console.Write(promt);
                }

                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Введите корректное число.");
            }
        }
    }

    private string GetStringInput(string? promt = null)
    {
        string? str;

        while (true)
        {
            try
            {
                if (promt is not null)
                {
                    Console.Write(promt);
                }

                str = Console.ReadLine();

                if (str is null || str.Equals(""))
                {
                    Console.WriteLine("Введите корректную строку.");
                }
                else
                {
                    return str;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Введите корректную строку.");
            }
        }
    }

    private string GetStringInputAllowEneter(string? promt = null)
    {
        string? str;

        while (true)
        {
            try
            {
                if (promt is not null)
                {
                    Console.Write(promt);
                }

                str = Console.ReadLine();

                if (str is null)
                {
                    Console.WriteLine("Введите корректную строку.");
                }
                else
                {
                    return str;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Введите корректную строку.");
            }
        }
    }

    private UserType GetUserTypeInput(string? promt = null)
    {
        while (true)
        {
            if (promt is null)
            {
                ShowUserTypes();
            }
            else
            {
                Console.Write(promt);
            }

            int typeNumber = GetIntInput("Выберите пункт: ");

            switch (typeNumber)
            {
                case 1:
                    return UserType.STUDENT;
                case 2:
                    return UserType.FACULTY;
                case 3:
                    return UserType.GUEST;
                default:
                    Console.WriteLine("Введите корректный пункт.");
                    break;
            }
        }
    }
}
