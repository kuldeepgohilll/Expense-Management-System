using ExpenseManagementSystem.Data;
using ExpenseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementSystem.Controllers;
public class TransactionsController : Controller
{
    private readonly AppDbContext _db;
    public TransactionsController(AppDbContext db) => _db = db;
    private int? UserId => HttpContext.Session.GetInt32("UserId");

    public async Task<IActionResult> Index(string? q, string? type, string? category)
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        var query = _db.Transactions.Where(x => x.UserId == UserId.Value).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Category.Contains(q) || x.Description.Contains(q));
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(x => x.Type == type);
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
        return View(await query.OrderByDescending(x => x.TransactionDate).ThenByDescending(x => x.Id).ToListAsync());
    }

    public IActionResult Create()
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        return View(new Transaction { TransactionDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Transaction tx)
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid) return View(tx);
        tx.UserId = UserId.Value;
        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Transaction added successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        var tx = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId.Value);
        return tx == null ? NotFound() : View(tx);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Transaction tx)
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid) return View(tx);
        var existing = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == tx.Id && x.UserId == UserId.Value);
        if (existing == null) return NotFound();
        existing.Type=tx.Type; existing.Amount=tx.Amount; existing.Category=tx.Category;
        existing.PaymentMethod=tx.PaymentMethod; existing.TransactionDate=tx.TransactionDate; existing.Description=tx.Description;
        await _db.SaveChangesAsync();
        TempData["Message"]="Transaction updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (UserId == null) return RedirectToAction("Login", "Account");
        var tx = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId.Value);
        if (tx != null) { _db.Transactions.Remove(tx); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
