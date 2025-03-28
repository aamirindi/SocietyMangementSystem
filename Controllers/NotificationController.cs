using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using System.Net.Http;

namespace SocietyMVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicatioDbContext db;
        private readonly IHttpContextAccessor httpContext;


        public NotificationController(ApplicatioDbContext db, IHttpContextAccessor httpContext)
        {
            this.db = db;
            this.httpContext = httpContext;
        }

        public ActionResult Index()
        {
            // Ensure UserId exists in session before parsing
            string userIdString = httpContext.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return PartialView("_NotificationList", new List<NotificationModel>()); // Return empty list
            }

            int userId = int.Parse(userIdString);

            // Fetch notifications for the logged-in user
            var notifications = db.notification // Ensure it's `Notifications`, not `notification`
                                  .Where(n => n.UserId == userId)
                                  
                                  .OrderByDescending(n => n.CreatedDate)
                                  .ToList();

            return PartialView("_NotificationList", notifications);
           // return View(notifications);
        }

    }
}
