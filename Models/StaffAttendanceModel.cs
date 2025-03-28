using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class StaffAttendanceModel
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan? CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }

        [NotMapped]
        public TimeSpan? TotalWorkedHours =>
            (CheckInTime.HasValue && CheckOutTime.HasValue)
            ? CheckOutTime - CheckInTime
            : null;

        public string AttendanceStatus
        {
            get
            {
                if (!CheckInTime.HasValue) return "Absent";
                if (!CheckOutTime.HasValue) return "Pending Checkout";
                return (TotalWorkedHours.HasValue && TotalWorkedHours.Value.TotalHours >= 6) ? "Full Day" : "Half Day";
            }
        }
    }
}
