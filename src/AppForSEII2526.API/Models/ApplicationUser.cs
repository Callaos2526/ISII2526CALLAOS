using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }

    public ApplicationUser(int id, string nombre, string apellido1, string apellido2)
    {
        Id = id;
        Nombre = nombre;
        Apellido1 = apellido1;
        Apellido2 = apellido2;
    }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido1 { get; set; }
    public string Apellido2 { get; set; }
}