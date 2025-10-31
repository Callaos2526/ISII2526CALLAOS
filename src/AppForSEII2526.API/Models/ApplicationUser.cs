using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }
    public ApplicationUser(string id, string name, string surname1, string surname2)
    {
        Id = id;
        Name = name;
        Surname1 = surname1;
        Surname2 = surname2;
        
    }

    [Display(Name = "Name")]
    public string Name {  get; set; }

    [Display(Name = "Surname1")]
    public string Surname1 {  get; set; }
    [Display(Name = "Surname2")]
    public string? Surname2 { get; set; }

    
}