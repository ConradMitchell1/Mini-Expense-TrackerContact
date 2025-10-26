using Microsoft.AspNetCore.Mvc;
using Mini_Expense_Tracker.Models.ViewModels;
using Mini_Expense_Tracker.Services.Interfaces;

namespace Mini_Expense_Tracker.Controllers
{
    public class ContactController : Controller
    {
        private readonly INotificationService _notificationService;
        public ContactController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactInput());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactInput input)
        {
            if(!ModelState.IsValid)
            {
                return View(input);
            }

            var content = $"Name: {input.Name}\nEmail: {input.Email}\n\nMessage:\n{input.Message}";

            await _notificationService.NotifyAsync(content);
            ViewBag.Success = true;
            return View(new ContactInput());
        }
    }
}
