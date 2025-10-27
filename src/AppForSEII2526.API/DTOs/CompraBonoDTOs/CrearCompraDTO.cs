using AppForSEII2526.API.Models;
using System.IO.Pipelines;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class CrearCompraDTO
    {
       // 5.	El sistema muestra la lista de los bonos seleccionados, incluyendo su precio individual, así como cuantos bocadillos incluye cada bono,
       // el nombre y tipo y pide al cliente que introduzca los siguientes datos: Nombre completo(el nombre al que estará el bono) , apellidos, y método de pago,
       // siendo todos estos campos obligatorios.Para cada bono, se pide una cantidad.
       public CrearCompraDTO(int compraId, ApplicationUser nombreCliente, ApplicationUser apellidoCliente1, ApplicationUser apellidoCliente2, DateTime fechaCompra, MetodoPago metodo, IList<BonoItemDTO> bonoItem)
        {
            CompraId = compraId;
            NombreCliente = nombreCliente;
            ApellidoCliente1 = apellidoCliente1;
            ApellidoCliente2 = apellidoCliente2;
            Metodo = metodo;
            FechaCompra = fechaCompra;
            BonoItem = bonoItem;
        }
        public CrearCompraDTO()
        {
            BonoItem = new List<BonoItemDTO>();
        }
        [Key]
        public int CompraId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede ser más largo a 50 caracteres")]
        public ApplicationUser NombreCliente { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "El apellido no puede ser más largo a 50 caracteres")]

        public ApplicationUser ApellidoCliente1 { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "El apellido no puede ser más largo a 50 caracteres")]
        public ApplicationUser ApellidoCliente2 { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        [Required]
        public MetodoPago Metodo { get; set; }
        public IList<BonoItemDTO> BonoItem { get; set;}

        [Display(Name = "Precio Total")]
        [JsonPropertyName("PrecioTotal")]
        public double PrecioTotal
        {
            get
            {
                return BonoItem.Sum(ri => ri.Precio * ri.NumeroDeBocadillos);
            }
        }



    }
}
