using System.ComponentModel.DataAnnotations;

namespace SocietyMVC.Models
{
    public class NoticeView
    {
        [Key]
        public int NoticeId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        public DateTime ExpiryDate { get; set; }

        public List<IFormFile> Attachments { get; set; } // For new file uploads

        public string AttachmentPaths { get; set; } // For existing file paths (comma-separated)

        public int CreatedBy { get; set; } // User ID from session
    }
}
