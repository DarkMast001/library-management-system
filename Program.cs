using LibraryManagementSystem.Entities;

namespace LibraryManagementSystem;

internal class Program
{
    static void Main(string[] args)
    {
        Library library = new("input.txt");

        LibraryConsole libraryConsole = new(library);

        libraryConsole.Run();
    }
}
