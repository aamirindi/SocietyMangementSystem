using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class NotificationModel
    {
        [Key]
        public int NotiId { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }//read or unread

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int UserId { get; set; } // FK Reference to Users (Admin)
        public UserModel User { get; set; }  // Many-to-One

        public String Billlink { get; set; }

    }
}
