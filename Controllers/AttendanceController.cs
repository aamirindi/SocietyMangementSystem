using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;

namespace SocietyMVC.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicatioDbContext _db;

        public AttendanceController(ApplicatioDbContext db)
        {
            _db = db;
        }

        // ✅ Staff Check-In (POST)
        [HttpPost]
        public IActionResult CheckIn()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            int loggedInUserId = int.Parse(userIdString);

            var existingRecord = _db.StaffAttendances.FirstOrDefault(a => a.UserId == loggedInUserId && a.Date == DateTime.Today);
            if (existingRecord != null) return RedirectToAction("MyAttendance");

            var attendance = new StaffAttendanceModel
            {
                UserId = loggedInUserId,
                Date = DateTime.Today,
                CheckInTime = DateTime.Now.TimeOfDay
            };

            _db.StaffAttendances.Add(attendance);
            _db.SaveChanges();

            return RedirectToAction("MyAttendance");
        }

        // ✅ Staff Check-Out (POST)
        [HttpPost]
        public IActionResult CheckOut()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            int loggedInUserId = int.Parse(userIdString);

            var attendance = _db.StaffAttendances.FirstOrDefault(a => a.UserId == loggedInUserId && a.Date == DateTime.Today);
            if (attendance == null || attendance.CheckOutTime.HasValue) return RedirectToAction("MyAttendance");

            attendance.CheckOutTime = DateTime.Now.TimeOfDay;
            _db.SaveChanges();

            return RedirectToAction("MyAttendance");
        }

        // ✅ Staff View Attendance
        public IActionResult MyAttendance()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            int loggedInUserId = int.Parse(userIdString);
            var attendanceRecords = _db.StaffAttendances
                .Where(a => a.UserId == loggedInUserId)
                .OrderByDescending(a => a.Date)
                .ToList();

            return View(attendanceRecords);
        }

        public IActionResult MarkAttendance()
        {
            return View();
        }

        // ✅ Admin View All Attendance
        public IActionResult AllAttendance()
        {
            var attendanceRecords = _db.StaffAttendances
                .Include(a => a.User)  // Ensure User data is loaded
                .OrderByDescending(a => a.Date)
                .ToList();

            return View(attendanceRecords);
        }
    }
}
