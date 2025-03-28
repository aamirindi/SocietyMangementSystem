using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class MaintenanceModel
    {
        [Key]
        public int BillId { get; set; }

        public String BillName { get; set; }


        [ForeignKey("User")]//ref variable from line no 14
        public int UserId { get; set; }   // FK
        public UserModel User { get; set; }    // Many-to-One

        [Required]
        public double Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string Status { get; set; } // Paid / Pending / Overdue

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public String IssuedBy { get; set; }

        public bool LateFeeApplied { get; set; }//true or False 
        // by default false kar 




    }
}