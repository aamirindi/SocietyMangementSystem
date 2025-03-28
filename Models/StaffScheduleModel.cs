using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class StaffScheduleModel
    {
      
            [Key]
            public int ScheduleId { get; set; }

            public int UserId { get; set; }  // Foreign Key to UserModel (Staff)
            [ForeignKey("UserId")]
            public UserModel User { get; set; }

            [Required]
            public string DayOfWeek { get; set; } // Example: "Monday"

            [Required]
            public TimeSpan ShiftStart { get; set; } // Example: 08:00

            [Required]
            public TimeSpan ShiftEnd { get; set; }   // Example: 17:00

            [Required]
            [StringLength(200)]
            public string Task { get; set; }  // Example: "Gate Duty", "Cleaning Block A"
        

    }
}
