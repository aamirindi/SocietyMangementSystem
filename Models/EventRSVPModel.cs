using SocietyMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class EventRSVPModel
{
    [Key]
    public int RSVPId { get; set; }

    // Foreign Key to Event
    [ForeignKey("Event")]

    public int EventId { get; set; }

    public EventMeetingModel Event { get; set; }

    // Foreign Key to User (Resident who RSVPs)
    [ForeignKey("User")]

    public int UserId { get; set; }

    public UserModel User { get; set; }

    [Required]
    [StringLength(10)]
    public string Response { get; set; } // Values: "Yes", "No", "Maybe"

    public DateTime RespondedOn { get; set; } = DateTime.Now;
}
