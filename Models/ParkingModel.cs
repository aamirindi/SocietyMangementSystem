using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace SocietyMVC.Models
{
    public class ParkingModel
    {
        // Constructor to initialize default values


        [Key]
        public int SlotId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public UserModel User { get; set; }

        [AllowNull]
        public string? VehicleNo { get; set; }

        [Required]
        public string Status { get; set; } // Occupied / Vacant

        [Required]
        public string VehicleCategory { get; set; } // Car, Bike, etc.

        [AllowNull]
        public string? OccupiedBy { get; set; } // Owner / Visitor

        [AllowNull]
        public DateTime? AssignedTime { get; set; }

        [Required]
        [AllowNull]
        public string? SlotNumber { get; set; }

        [AllowNull]
        public DateTime? ExitTime { get; set; }
    }
}