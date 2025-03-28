using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using SocietyMVC.Services;

namespace SocietyMVC.Controllers
{
    public class NoticeController : Controller
    {
        private readonly INotice _noticeService;
        private readonly ApplicatioDbContext _db;
        private readonly IWebHostEnvironment _env;

        public NoticeController(
            INotice noticeService,
            IWebHostEnvironment env,
            ApplicatioDbContext db)
        {
            _noticeService = noticeService;
            _env = env;
            _db = db;
        }

        public IActionResult NoticeIndex(string filter = "all")
        {
            var notices = _noticeService.GetAllNotices(filter);
            return View(notices);
        }

        public IActionResult UserNotice(string filter = "active")
        {
            var notices = _noticeService.GetAllNotices(filter);
            return View(notices);
        }
        public IActionResult CreateNotice()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNotice(NoticeView noticeView)
        {

            noticeView.CreatedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            _noticeService.AddNotice(noticeView, _env.WebRootPath);
            return RedirectToAction("NoticeIndex");

        }

        public IActionResult ViewNotice(int id)
        {
            var notice = _noticeService.GetNoticeById(id);
            if (notice == null || notice.ExpiryDate < DateTime.Now)
                return NotFound();

            return View(notice);
        }

        public IActionResult EditNotice(int id)
        {
            var notice = _noticeService.GetNoticeById(id);
            if (notice == null || notice.ExpiryDate < DateTime.Now)
                return NotFound();

            var noticeView = new NoticeView
            {
                NoticeId = notice.NoticeId,
                Title = notice.Title,
                Description = notice.Description,
                ExpiryDate = notice.ExpiryDate,
                AttachmentPaths = notice.AttachmentPaths, // Pass existing attachments
                CreatedBy = notice.CreatedBy
            };

            return View(noticeView);
        }

        [HttpPost]
        public IActionResult EditNotice(NoticeView noticeView)
        {

            _noticeService.UpdateNotice(noticeView, _env.WebRootPath);
            return RedirectToAction("NoticeIndex");

        }
        public IActionResult DeleteNotice(int id)
        {
            _noticeService.DeleteNotice(id);
            return RedirectToAction("NoticeIndex");
        }
    }
}
