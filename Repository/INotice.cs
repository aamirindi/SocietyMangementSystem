using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface INotice
    {
        List<NoticeModel> GetAllNotices(string filter = null);
        NoticeModel GetNoticeById(int id);
        void AddNotice(NoticeView noticeView, string webRootPath);
        void UpdateNotice(NoticeView noticeView, string webRootPath);
        void DeleteNotice(int id);
    }
}
