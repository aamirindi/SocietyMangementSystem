using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IStaffScheduleRepository
    {
        IEnumerable<StaffScheduleModel> GetAll();
        StaffScheduleModel GetById(int id);
        void Add(StaffScheduleModel schedule);
        void Update(StaffScheduleModel schedule);
        void Delete(int id);
        IEnumerable<StaffScheduleModel> Filter(string day, int? userId);
    }
}
