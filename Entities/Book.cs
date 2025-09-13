namespace LibraryManagementSystem.Entities;

internal record Book(string Title, string Author, string Isbn, string Genre)
{
    public bool IsAvailable { get; set; } = true;

    public DateTime BorrowTime { get; set; }

    public override string ToString()
    {
        string t = IsAvailable ? "Доступна" : "Выдана";
        return $"{Isbn} - {Title} - {Author} - {Genre} - {t}";
    }
}
