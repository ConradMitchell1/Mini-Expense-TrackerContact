using Microsoft.AspNetCore.Mvc;
using Mini_Expense_Tracker.Data;
using Mini_Expense_Tracker.Models.ViewModels;
using Mini_Expense_Tracker.Services.Interfaces;
using System.Threading.Tasks;

namespace Mini_Expense_Tracker.Controllers
{
    // Single responsibility: Only orchestrates HTTP flow
    public class ExpenseController : Controller
    {
        private readonly INotificationService _notifier;
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;
        private readonly IBudgetRule _budgetRule;

        public ExpenseController(IExpenseService expenseService, INotificationService notifier, ICategoryService categoryService, IBudgetRule budgetRule)
        {
            _expenseService = expenseService;
            _notifier = notifier;
            _categoryService = categoryService;
            _budgetRule = budgetRule;
        }
        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseService.GetAsync();

            var month = DateOnly.FromDateTime(DateTime.UtcNow);
            var status = await _budgetRule.EvaluateAsync(month);

            ViewBag.BudgetStatus = status;

            return View(expenses);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();
            var model = new ExpenseInput
            {
                Categories = categories.Select(c => new CategoryInput
                {
                    Id = c.Id,
                    Name = c.Name
                })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseInput model)
        {
            var result = await _expenseService.AddAsync(model.ToEntity());
            if (result.BudgetExceeded)
            {
                await _notifier.NotifyAsync("Budget exceeded for this month!");
            }
            else
            {
                await _notifier.NotifyAsync("Expense added successfully.");
            }



            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _expenseService.DeleteAsync(id);
            await _notifier.NotifyAsync("Expense deleted successfully.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export(string format = "csv")
        {
            var data = await _expenseService.ExportAsync(format);
            return File(data, "text/csv", $"expenses_{DateTime.UtcNow:yyyyMM}.{format}");
        }
    }
}
