using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;

namespace SocietyMVC.Services
{
    public class NoticeService : INotice
    {
        private readonly ApplicatioDbContext _db;

        public NoticeService(ApplicatioDbContext db)
        {
            _db = db;
        }

        public List<NoticeModel> GetAllNotices(string filter = "all")
        {
            var query = _db.Notice.AsQueryable();

            if (filter == "active")
                query = query.Where(n => n.ExpiryDate >= DateTime.Now);
            else if (filter == "expired")
                query = query.Where(n => n.ExpiryDate < DateTime.Now);

            return query.ToList();
        }

        public NoticeModel GetNoticeById(int id)
        {
            return _db.Notice.Find(id);
        }

        public void AddNotice(NoticeView noticeView, string webRootPath)
        {
            var attachmentPaths = SaveAttachments(noticeView.Attachments, webRootPath);

            var notice = new NoticeModel
            {
                Title = noticeView.Title,
                Description = noticeView.Description,
                ExpiryDate = noticeView.ExpiryDate,
                AttachmentPaths = string.Join(",", attachmentPaths),
                CreatedBy = noticeView.CreatedBy
            };

            _db.Notice.Add(notice);
            _db.SaveChanges();
        }

        public void UpdateNotice(NoticeView noticeView, string webRootPath)
        {
            var notice = _db.Notice.Find(noticeView.NoticeId);
            if (notice != null)
            {
                notice.Title = noticeView.Title;
                notice.Description = noticeView.Description;
                notice.ExpiryDate = noticeView.ExpiryDate;

                if (noticeView.Attachments != null && noticeView.Attachments.Count > 0)
                {
                    var attachmentPaths = SaveAttachments(noticeView.Attachments, webRootPath);
                    notice.AttachmentPaths = string.Join(",", attachmentPaths);
                }

                _db.SaveChanges();
            }
        }

        public void DeleteNotice(int id)
        {
            var notice = _db.Notice.Find(id);
            if (notice != null)
            {
                _db.Notice.Remove(notice);
                _db.SaveChanges();
            }
        }

        private List<string> SaveAttachments(List<IFormFile> attachments, string webRootPath)
        {
            var attachmentPaths = new List<string>();
            if (attachments != null && attachments.Count > 0)
            {
                string uploadFolder = Path.Combine(webRootPath, "Attachments");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var file in attachments)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    attachmentPaths.Add(uniqueFileName);
                }
            }
            return attachmentPaths;
        }
    }
}
