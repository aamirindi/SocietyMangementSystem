using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class StaffModel
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }                        // Security / Cleaner /Plumber/ etc.

        public double Salary { get; set; }                      //monthly Salary

        // Optional One-to-One link to User for login purposes

        //[ForeignKey("User")]

        //public int? UserId { get; set; }
        //public UserModel User { get; set; }

    }





}
