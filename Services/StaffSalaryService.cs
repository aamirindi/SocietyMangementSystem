using SocietyMVC.Data;
using SocietyMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace SocietyMVC.Services
{
    public class StaffSalaryService
    {
        private readonly ApplicatioDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffSalaryService(ApplicatioDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Get Salary List for Admin
        public List<StaffSalaryModel> GetAllSalaries()
        {
            string role = _httpContextAccessor.HttpContext.Session.GetString("MyRole");

            if (role == "Admin")
            {
                return _context.StaffSalaries.Include(s => s.User).OrderByDescending(s => s.Month).ToList();
            }

            return new List<StaffSalaryModel>();
        }

        // ✅ Get Salary for Logged-in Staff
        public List<StaffSalaryModel> GetSalariesForCurrentUser()
        {
            string role = _httpContextAccessor.HttpContext.Session.GetString("MyRole");

            if (role == "Staff")
            {
                int userId = int.Parse(_httpContextAccessor.HttpContext.Session.GetString("UserId"));
                return _context.StaffSalaries.Where(s => s.UserId == userId).OrderByDescending(s => s.Month).ToList();
            }

            return new List<StaffSalaryModel>();
        }

        // ✅ Calculate and Process Salary
        public void ProcessSalary(string month)
        {
            var staffList = _context.Users.Where(u => u.Role == "Staff").ToList();
            var existingSalaries = _context.StaffSalaries.Where(s => s.Month == month).ToList();

            foreach (var staff in staffList)
            {
                if (existingSalaries.Any(s => s.UserId == staff.UserId)) continue;

                // Fetch attendance records for the given month (fix applied)
                var attendanceRecords = _context.StaffAttendances
                    .Where(a => a.UserId == staff.UserId)
                    .AsEnumerable()  // Moves processing to memory
                    .Where(a => a.Date.ToString("MMMM-yyyy") == month)
                    .ToList();

                int totalPresent = attendanceRecords.Count(a => a.AttendanceStatus == "Full Day");
                int totalAbsent = 30 - totalPresent;
                decimal salary = 15000m * (totalPresent / 30m);

                var salaryRecord = new StaffSalaryModel
                {
                    UserId = staff.UserId,
                    Month = month,
                    TotalPresent = totalPresent,
                    TotalAbsent = totalAbsent,
                    CalculatedSalary = salary,
                    PaymentStatus = "Unpaid"
                };

                _context.StaffSalaries.Add(salaryRecord);
            }

            _context.SaveChanges();
        }


        // ✅ Mark Salary as Paid
        public void MarkAsPaid(int salaryId)
        {
            var salary = _context.StaffSalaries.Find(salaryId);
            if (salary != null)
            {
                salary.PaymentStatus = "Paid";
                _context.SaveChanges();
            }
        }
    }
}
