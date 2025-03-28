using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;


namespace SocietyMVC.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Flats = new List<FlatModel>();
            Maintenance = new List<MaintenanceModel>();
            Complain = new List<ComplainModel>();
            EventMeeting = new List<EventMeetingModel>();
            Visitors = new List<VisitorsModel>();
            Parking = new List<ParkingModel>();
            Notice = new List<NoticeModel>();
        }

        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        // Single field for Flat Number
        public string? FlatNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // ✅ One-to-Many Relations
        public ICollection<MaintenanceModel> Maintenance { get; set; }
        public ICollection<ComplainModel> Complain { get; set; }
        public ICollection<EventMeetingModel> EventMeeting { get; set; }
        public ICollection<VisitorsModel> Visitors { get; set; }
        public ICollection<ParkingModel> Parking { get; set; }
        public ICollection<NoticeModel> Notice { get; set; }
        public ICollection<EventRSVPModel> EventRSVP { get; set; }
        public ICollection<FlatModel> Flats { get; set; }
    }
}

