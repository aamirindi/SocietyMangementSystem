using Microsoft.EntityFrameworkCore;
using SocietyMVC.Data;
using SocietyMVC.Models;
using SocietyMVC.Repository;


namespace SocietyMVC.Services
{
    public class StaffScheduleService : IStaffScheduleRepository
    {
        private readonly ApplicatioDbContext _db;
        public StaffScheduleService(ApplicatioDbContext db) { _db = db; }

        public IEnumerable<StaffScheduleModel> GetAll()
        {
            return _db.StaffSchedules.Include(s => s.User).ToList();
        }

        public StaffScheduleModel GetById(int id)
        {
            return _db.StaffSchedules.Include(s => s.User).FirstOrDefault(s => s.ScheduleId == id);
        }

        public void Add(StaffScheduleModel schedule)
        {
            _db.StaffSchedules.Add(schedule);
            _db.SaveChanges();
        }

        public void Update(StaffScheduleModel schedule)
        {
            _db.StaffSchedules.Update(schedule);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var data = GetById(id);
            if (data != null)
            {
                _db.StaffSchedules.Remove(data);
                _db.SaveChanges();
            }
        }

        public IEnumerable<StaffScheduleModel> Filter(string day, int? userId)
        {
            var query = _db.StaffSchedules.AsQueryable();

            // Ensure that the StaffSchedules table is not null
            if (query == null) return new List<StaffScheduleModel>();

            // Apply filters
            if (!string.IsNullOrEmpty(day))
                query = query.Where(s => s.DayOfWeek.ToLower() == day.ToLower());

            if (userId.HasValue)
                query = query.Where(s => s.UserId == userId.Value);

            return query.ToList();
        }

    }

}
