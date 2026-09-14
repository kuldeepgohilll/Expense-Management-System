using ExpenseManagementSystem.Models;
namespace ExpenseManagementSystem.ViewModels;
public class DashboardViewModel
{
    public string Month { get; set; } = "";
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal Budget { get; set; }
    public List<Transaction> RecentTransactions { get; set; } = new();
    public Dictionary<string, decimal> CategoryTotals { get; set; } = new();
    public decimal Balance => Income - Expenses;
    public decimal BudgetUsed => Budget > 0 ? Math.Min(Expenses / Budget * 100, 100) : 0;
}
