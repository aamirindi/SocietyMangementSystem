using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class ComplainModel
    {

        [Key]
        public int ComplaintId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public UserModel User { get; set; }  // Many-to-One

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; } = "Open";

        public string AdminResponse { get; set; } = "No Response Yet";
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        

    }
}
