#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class WeddingGuest
{
    [Key]
    public int WeddingGuestId { get; set; }

    [Required]
    //Primary Key and References of both other classes
    public int UserId { get; set; }
    public User? InvitedGuest { get; set; }

    [Required]
    public int WeddingId { get; set; }
    public Wedding? WeddingEvent { get; set; }

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}