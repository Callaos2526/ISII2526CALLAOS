using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public string Name { get; set; }
    public string Surname1 { get; set; }
    public string Sruename2 { get; set; }
}