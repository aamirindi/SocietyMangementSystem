using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public enum VisitorStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class VisitorsModel
    {
        [Key]
        public int VisitorId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FlatNumber { get; set; }

        public DateTime EntryTime { get; set; } = DateTime.Now;
        public DateTime? ExitTime { get; set; }

        [ForeignKey("ApprovedUser")]
        public int UserId { get; set; } // FK to User
        public UserModel ApprovedUser { get; set; }

        [Required]
        public bool HasVehicle { get; set; } // 1 - yes, 0 - no (Displayed after approval)

        public string? SlotNo { get; set; } // If HasVehicle == true, assign a slot

        public VisitorStatus Status { get; set; } = VisitorStatus.Pending; // Default Status

        public DateTime? OtpExpiry { get; set; } // OTP expiration time
    }
}
