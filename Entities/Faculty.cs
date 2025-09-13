namespace LibraryManagementSystem.Entities;

internal class Faculty : User
{
    public Faculty(string name, string userId, string email) : base(name, userId, email) { }

    public override string Name { get; set; } = string.Empty;
    public override string UserId { get; set; } = string.Empty;
    public override string Email { get; set; } = string.Empty;

    public override int GetMaxBooks()
    {
        return 10;
    }

    public override int GetBorrowDays()
    {
        return 30;
    }

    public override double GetFinePerDay()
    {
        return 0.10;
    }
}
