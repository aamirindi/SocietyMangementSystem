using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocietyMVC.Models
{
    public class FlatModel
    {
        [Key]
        public int FlatId { get; set; }
        public string FlatNumber { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public UserModel User { get; set; }
    }
}