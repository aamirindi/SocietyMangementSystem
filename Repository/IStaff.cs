using SocietyMVC.Models;

namespace SocietyMVC.Repository
{
    public interface IStaff
    {
        void AddStaff(StaffModel staff);
        IEnumerable<StaffModel> GetAllStaff();
        StaffModel GetStaffById(int staffId);

    }
}
