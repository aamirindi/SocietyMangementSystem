using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using SocietyMVC.Services;

namespace SocietyMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffScheduleRepository _service;
        private readonly ApplicatioDbContext _db;

        public StaffController(IStaffScheduleRepository service, ApplicatioDbContext db)
        {
            _service = service;
            _db = db;
        }

        public IActionResult Index(string day, int? userId)
        {
            ViewBag.Users = new SelectList(_db.Users.Where(u => u.Role == "Staff"), "UserId", "Name");
            var data = _service.Filter(day, userId);
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_db.Users.Where(u => u.Role == "Staff"), "UserId", "Name");
            return View(new StaffScheduleModel());  // Always pass a new object
        }


        [HttpPost]
        public IActionResult Create(StaffScheduleModel model)
        {
            
                _service.Add(model);
                return RedirectToAction("Index");
            
            //ViewBag.Users = new SelectList(_db.Users.Where(u => u.Role == "Staff"), "UserId", "Name");
            //return View(model);
        }

        public IActionResult Edit(int id)
        {
            var data = _service.GetById(id);
            ViewBag.Users = new SelectList(_db.Users.Where(u => u.Role == "Staff"), "UserId", "Name", data.UserId);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(StaffScheduleModel model)
        {
                _service.Update(model);
                return RedirectToAction("Index");
            
            //ViewBag.Users = new SelectList(_db.Users.Where(u => u.Role == "Staff"), "UserId", "Name", model.UserId);
            //return View(model);
        }

        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult MyTasks()
        {
            // Retrieve UserId from session
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Redirect if session is expired or user not logged in
            }

            int loggedInUserId = int.Parse(userIdString); // Convert to integer

            var tasks = _service.GetAll()
                                .Where(s => s.UserId == loggedInUserId)
                                .ToList();

            return View(tasks);
        }

    }

}
