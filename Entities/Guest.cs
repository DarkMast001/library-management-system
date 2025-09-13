namespace LibraryManagementSystem.Entities;

internal class Guest : User
{
    public Guest(string name, string userId, string email) : base(name, userId, email) { }

    public override string Name { get; set; } = string.Empty;
    public override string UserId { get; set; } = string.Empty;
    public override string Email { get; set; } = string.Empty;

    public override int GetMaxBooks()
    {
        return 1;
    }

    public override int GetBorrowDays()
    {
        return 7;
    }

    public override double GetFinePerDay()
    {
        return 0.80;
    }
}
