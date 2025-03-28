using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Services;

namespace SocietyMVC.Controllers
{
    public class StaffSalaryController : Controller
    {
        private readonly StaffSalaryService _salaryService;

        public StaffSalaryController(StaffSalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        // ✅ Admin View All Salaries
        public IActionResult Index()
        {
            var salaries = _salaryService.GetAllSalaries();
            return View(salaries);
        }

        // ✅ Staff View Their Salary
        public IActionResult MySalary()
        {
            var salaries = _salaryService.GetSalariesForCurrentUser();
            return View(salaries);
        }

        // ✅ Process Salary Calculation
        public IActionResult ProcessSalary(string month)
        {
            _salaryService.ProcessSalary(month);
            return RedirectToAction("Index");
        }

        // ✅ Mark Salary as Paid
        public IActionResult MarkAsPaid(int salaryId)
        {
            _salaryService.MarkAsPaid(salaryId);
            return RedirectToAction("Index");
        }
    }

}