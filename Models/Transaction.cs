using System.ComponentModel.DataAnnotations;
namespace ExpenseManagementSystem.Models;
public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    [Required] public string Type { get; set; } = "Expense";
    [Range(0.01, 100000000)] public decimal Amount { get; set; }
    [Required] public string Category { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    [DataType(DataType.Date)] public DateTime TransactionDate { get; set; } = DateTime.Today;
    public string Description { get; set; } = "";
}
