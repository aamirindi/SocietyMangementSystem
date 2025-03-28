using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class EventMeetingModel
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Venue { get; set; }

        [ForeignKey("CreatedUser")]
        public int CreatedBy { get; set; }  // FK (Reference to Users (Admin))
        public UserModel CreatedUser { get; set; }  // Many-to-One

    }
}
