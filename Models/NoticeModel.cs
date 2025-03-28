using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class NoticeModel
    {
        [Key]
        public int NoticeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? AttachmentPaths { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int CreatedBy { get; set; }  // FK Reference to Users (Admin)


    }
}
