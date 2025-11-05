using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser() { }
       
    public ApplicationUser(int id, string name, string surname1, string? surname2, string email, string direccion)
    {
        Id = id;
        Name = name;
        Surname1= surname1;
        Surname2 = surname2;
        
    }
   public int Id {  get; set; }
   
    public string Name {  get; set; }

    public string Surname1 {  get; set; }
   
    public string? Surname2 { get; set; }
   

    
}