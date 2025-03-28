using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;
using SocietyMVC.Data;
using Microsoft.EntityFrameworkCore;
namespace SocietyMVC.Services
{
    public class StaffService : IStaff
    {
        private readonly ApplicatioDbContext _context;

        public StaffService(ApplicatioDbContext context)
        {
            _context = context;
        }

        public void AddStaff(StaffModel staff)
        {
            _context.Staff.Add(staff);
            _context.SaveChanges();
        }

        public IEnumerable<StaffModel> GetAllStaff()
        {
            return _context.Staff.ToList();
        }

        public StaffModel GetStaffById(int staffId)
        {
            return _context.Staff.Find(staffId);
        }



    }
}
