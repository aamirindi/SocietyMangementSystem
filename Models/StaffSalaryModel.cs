using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StaffSalaryModel
    {
        [Key]
        public int SalaryId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        [Required]
        public string Month { get; set; } // Example: "March-2025"

        [Required]
        public int TotalPresent { get; set; }

        [Required]
        public int TotalAbsent { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")] // ✅ Fix Precision Issue
        public decimal CalculatedSalary { get; set; }

        [Required]
        public string PaymentStatus { get; set; } // "Paid" or "Unpaid"

    }



}

