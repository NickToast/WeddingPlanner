#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;
public class User
{        
    [Key]        
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "First name is required to register")]        
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required to register")]      
    [Display(Name = "Last Name")]  
    public string LastName { get; set; }         
    
    [Required(ErrorMessage = "Email is required to register")]
    [EmailAddress]
    [UniqueEmail(ErrorMessage = "Email is already taken")]
    public string Email { get; set; }        
    
    [Required(ErrorMessage = "Password is required to register")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }          
    
    
    // This does not need to be moved to the bottom
    // But it helps make it clear what is being mapped and what is not
    [NotMapped]
    // There is also a built-in attribute for comparing two fields we can use!
    [Compare("Password", ErrorMessage = "Confirm password must match password")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    //One to Many
    public List<Wedding> CreatedWeddings { get; set; } = new List<Wedding>();

    //Many to Many
    public List<WeddingGuest> UserWeddings { get; set; } = new List<WeddingGuest>();
    
}


public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Though we have Required as a validation, sometimes we make it here anyways
        // In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
            // If it was, return the required error
            return new ValidationResult("Email is required!");
        }
    
        // This will connect us to our database since we are not in our Controller
        MyContext db = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if(db.Users.Any(e => e.Email == value.ToString()))
        {
            // If yes, throw an error
            return new ValidationResult("Email must be unique!");
        } else {
            // If no, proceed
            return ValidationResult.Success;
        }
    }
}
