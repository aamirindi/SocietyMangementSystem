using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocietyMVC.Models;

namespace SocietyMVC.Data
{
    public class ApplicatioDbContext : DbContext
    {
        public ApplicatioDbContext(DbContextOptions<ApplicatioDbContext> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<MaintenanceModel> Maintenance { get; set; }
        public DbSet<ComplainModel> Complain { get; set; }
        public DbSet<VisitorsModel> Visitors { get; set; }
        public DbSet<EventMeetingModel> EventMeeting { get; set; }
        public DbSet<ParkingModel> Parking { get; set; }
        public DbSet<NoticeModel> Notice { get; set; }
        public DbSet<StaffModel> Staff { get; set; }
        public DbSet<EventRSVPModel> EventRSVP { get; set; }
        public DbSet<NotificationModel> notification { get; set; }
        public DbSet<StaffScheduleModel> StaffSchedules { get; set; }
        public DbSet<StaffAttendanceModel> StaffAttendances { get; set; }
        public DbSet<StaffSalaryModel> StaffSalaries { get; set; }
        public DbSet<FlatModel> Flats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventRSVPModel>()
                .HasOne(e => e.User)
                .WithMany(u => u.EventRSVP)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }


    }
}
