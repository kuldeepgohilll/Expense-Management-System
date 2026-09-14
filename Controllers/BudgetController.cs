using ExpenseManagementSystem.Data;
using ExpenseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementSystem.Controllers;
public class BudgetController : Controller
{
    private readonly AppDbContext _db;
    public BudgetController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? month)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToAction("Login", "Account");
        month ??= DateTime.Today.ToString("yyyy-MM");
        var budget = await _db.Budgets.FirstOrDefaultAsync(x => x.UserId == uid && x.Month == month);
        var y=int.Parse(month[..4]); var m=int.Parse(month[5..7]);
        var spent=await _db.Transactions.Where(x=>x.UserId==uid && x.Type=="Expense" && x.TransactionDate.Year==y && x.TransactionDate.Month==m).SumAsync(x=>(decimal?)x.Amount) ?? 0;
        return View((month, budget?.Amount ?? 0, spent));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(string month, decimal amount)
    {
        var uid=HttpContext.Session.GetInt32("UserId");
        if(uid==null) return RedirectToAction("Login","Account");
        var b=await _db.Budgets.FirstOrDefaultAsync(x=>x.UserId==uid && x.Month==month);
        if(b==null) _db.Budgets.Add(new Budget{UserId=uid.Value,Month=month,Amount=Math.Max(0,amount)});
        else b.Amount=Math.Max(0,amount);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index),new{month});
    }
}
