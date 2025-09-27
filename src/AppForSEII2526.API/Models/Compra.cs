using System.Runtime.InteropServices;

namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra(string apellido_1Cliente, string apellido_2Cliente, int compraId, DateTime fechaCompra, int nbocadillos, string nombreCliente, int precioTotal) { 
            Apellido_1Cliente = apellido_1Cliente;
            Apellido_2Cliente = apellido_2Cliente;
            CompraId = compraId;
            FechaCompra = fechaCompra;
            nBocadillos = nbocadillos;
            NombreCliente = nombreCliente;
            PrecioTotal = precioTotal;
        }


        [Required]
        public string Apellido_1Cliente { get; set; }
        
        public string Apellido_2Cliente { get; set; }
        [Key]
        public int CompraId { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; }
        [Required]
        public int nBocadillos { get; set; }
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public int PrecioTotal { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Compra compra &&
                   Apellido_1Cliente == compra.Apellido_1Cliente &&
                   Apellido_2Cliente == compra.Apellido_2Cliente &&
                   CompraId == compra.CompraId &&
                   FechaCompra == compra.FechaCompra &&
                   nBocadillos == compra.nBocadillos &&
                   NombreCliente == compra.NombreCliente &&
                   PrecioTotal == compra.PrecioTotal;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Apellido_1Cliente, Apellido_2Cliente, CompraId, FechaCompra, nBocadillos, NombreCliente, PrecioTotal);
        }
    }
}
