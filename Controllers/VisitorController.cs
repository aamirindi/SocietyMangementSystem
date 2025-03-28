using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocietyMVC.Controllers
{
    public class VisitorController : Controller
    {
        private readonly ApplicatioDbContext db;
        private readonly IVisitor vs;

        public VisitorController(ApplicatioDbContext db, IVisitor vs)
        {
            this.db = db;
            this.vs = vs;
        }

        [HttpGet]
        public async Task<IActionResult> AddVisitor()
        {
            ViewBag.Users = db.Users
                .Where(u => u.FlatNumber != null)
                .Select(u => new { u.UserId, u.FlatNumber })
                .ToList();

            // Fetch all visitors, regardless of status
            var allVisitors = await db.Visitors.ToListAsync();

            var viewModel = new VisitorViewModel
            {
                NewVisitor = new VisitorsModel(),
                ApprovedVisitors = allVisitors // Renamed to reflect all visitors
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("RequestVisitorOTP")]
        public async Task<IActionResult> RequestVisitorOTP([FromForm] VisitorViewModel model)
        {
            try
            {
                if (model == null || model.NewVisitor == null)
                {
                    return Json(new { success = false, message = "Invalid visitor data." });
                }

                int visitorId = await vs.RequestVisitorOTP(model.NewVisitor);

                if (visitorId > 0)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Failed to request OTP." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }

        [HttpPost]
        [Route("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(string visitorEmail, string enteredOtp)
        {
            bool isVerified = await vs.VerifyOTP(visitorEmail, enteredOtp);
            if (isVerified)
            {
                TempData["Success"] = "Visitor approved successfully.";
                return RedirectToAction("AddVisitor");
            }
            else
            {
                TempData["Error"] = "Invalid OTP. Please try again.";
                return RedirectToAction("AddVisitor");
            }
        }

        [HttpPost]
        [Route("ResendOTP")]
        public async Task<IActionResult> ResendOTP([FromBody] string email)
        {
            bool success = await vs.ResendOTP(email);
            return Json(new { success });
        }

        [HttpGet]
        [Route("GetApprovedVisitors")]
        public async Task<IActionResult> GetApprovedVisitors()
        {
            var approvedVisitors = await db.Visitors
                .Where(v => v.Status == VisitorStatus.Approved)
                .ToListAsync();

            return PartialView("_ApprovedVisitorsTable", approvedVisitors);
        }

        [HttpPost]
        [Route("DeleteVisitor")]
        public async Task<IActionResult> DeleteVisitor([FromBody] string email)
        {
            bool success = await vs.DeleteVisitor(email);
            return Json(new { success });
        }

        public IActionResult DeleteVisitorBtn(int id)
        {
            vs.DeleteVisitorBtn(id);
            TempData["error"] = "Visitor Deleted Successfully";
            return RedirectToAction("AddVisitor");
        }

        public IActionResult UpdateExitTime(int id)
        {
            vs.UpdateExitTime(id);
            TempData["success"] = "Exit Time Updated Successfully";
            return RedirectToAction("AddVisitor");
        }

        public IActionResult RejectedVisitor(int id)
        {
            vs.RejectedVisitor(id);
            TempData["error"] = "Visitor Rejected Successfully";
            return RedirectToAction("AddVisitor");
        }
    }
}
