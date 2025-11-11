using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }

    public ApplicationUser(string nombreCliente, string apellidoCliente1, string apellidoCliente2)
    {
        NombreCliente = nombreCliente;
        ApellidoCliente1 = apellidoCliente1;
        ApellidoCliente2 = apellidoCliente2;
    }
   // public int ID { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente1 { get; set; }
    public string ApellidoCliente2 { get; set; }
    
}