using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SocietyMVC.Data;
using SocietyMVC.Models;

namespace SocietyMVC.Controllers;

public class HomeController : Controller
{
    private readonly ApplicatioDbContext db;
    private readonly ILogger<HomeController> _logger;

    // Only one constructor should exist
    public HomeController(ILogger<HomeController> logger, ApplicatioDbContext db)
    {
        _logger = logger;
        this.db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult AdminDashboard()
    {
        return View();
    }

    // Total Counts --start
    [HttpGet]
    public IActionResult GetTotalResident()
    {
        int data = db.Users.Count(u => u.Role.Equals("Resident"));
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalFlat()
    {
        int data = db.Users.Count();
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalVisitor()
    {
        int data = db.Visitors.Count();
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalParkingRequest()
    {
        int data = db.Parking.Count();
        return Json(data);
    }
    [HttpGet]
    public IActionResult GetTotalCompliant()
    {
        int data = db.Complain.Count();
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalStaff()
    {
        int data = db.Users.Count(u => u.Role.Equals("Staff"));
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalEvent()
    {
        int data = db.EventMeeting.Count();
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalNotice()
    {
        int data = db.Notice.Count();
        return Json(data);
    }

    [HttpGet]
    public IActionResult GetTotalBill()
    {
        int data = db.Maintenance.Count();
        return Json(data);
    }
    // Total Counts -- end

    // User Chart  --start
    [HttpGet]
    public IActionResult GetUsersByRole()
    {
        var roleData = db.Users
            .GroupBy(u => u.Role)
            .Select(g => new
            {
                Role = g.Key,
                Count = g.Count()
            }).ToList();

        return Json(roleData);
    }
    // User Chart  --end

    public IActionResult ResidentDashboard()
    {
        return View();
    }
    public IActionResult StaffDashboard()
    {
        return View();
    }

    public IActionResult ViewUser()
    {
        return View();
    }

}
