namespace LibraryManagementSystem.Entities;

internal class Student : User
{
    public Student(string name, string userId, string email) : base(name, userId, email) { }

    public override string Name { get; set; } = string.Empty;
    public override string UserId { get; set; } = string.Empty;
    public override string Email { get; set; } = string.Empty;

    public override int GetMaxBooks()
    {
        return 3;
    }

    public override int GetBorrowDays()
    {
        return 14;
    }

    public override double GetFinePerDay()
    {
        return 0.50;
    }
}
