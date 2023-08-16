#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Forms;

namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    //Destination, description, ImageUrl, Spring/Summer/Fall/Winter checkboxes

    [Required]
    [MinLength(2, ErrorMessage = "Name must have at least minimum 2 characters")]
    [Display(Name = "Wedder One")]
    public string WedderOne { get; set; }


    [Required]
    [MinLength(2, ErrorMessage = "Name must have at least minimum 2 characters")]
    [Display(Name = "Wedder Two")]
    public string WedderTwo { get; set; }

    [Required]
    [Display(Name = "Wedding Date")]
    [FutureDate]
    public DateTime WeddingDate { get; set; }

    [Required]
    public string Address{ get; set; }

    //Foreign Key - One to Many
    public int UserId { get; set; }
    public User? Creator { get; set; }

    //Many to Many
    public List<WeddingGuest> Guests { get; set; } = new List<WeddingGuest>();

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        DateTime Now = DateTime.Now;
        DateTime Input = (DateTime)value;

        if (Input < Now)
        {
            return new ValidationResult("Wedding date must be in the future");
        }
        else 
        {
            return ValidationResult.Success;
        }
    }
}