using ExpenseManagementSystem.Data;
using ExpenseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementSystem.Controllers;
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? month)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToAction("Login", "Account");
        month ??= DateTime.Today.ToString("yyyy-MM");

        var y = int.Parse(month[..4]); var m = int.Parse(month[5..7]);
        var tx = await _db.Transactions.Where(x => x.UserId == uid && x.TransactionDate.Year == y && x.TransactionDate.Month == m).ToListAsync();
        var budget = await _db.Budgets.FirstOrDefaultAsync(x => x.UserId == uid && x.Month == month);

        var vm = new DashboardViewModel {
            Month = month,
            Income = tx.Where(x => x.Type == "Income").Sum(x => x.Amount),
            Expenses = tx.Where(x => x.Type == "Expense").Sum(x => x.Amount),
            Budget = budget?.Amount ?? 0,
            RecentTransactions = await _db.Transactions.Where(x => x.UserId == uid)
                .OrderByDescending(x => x.TransactionDate).ThenByDescending(x => x.Id).Take(8).ToListAsync(),
            CategoryTotals = tx.Where(x => x.Type == "Expense")
                .GroupBy(x => x.Category).OrderByDescending(g => g.Sum(x => x.Amount))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount))
        };
        return View(vm);
    }
}
