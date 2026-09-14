namespace ExpenseManagementSystem.Models;
public class Budget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Month { get; set; } = "";
    public decimal Amount { get; set; }
}
