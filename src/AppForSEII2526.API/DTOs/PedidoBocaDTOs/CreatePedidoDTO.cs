using AppForSEII2526.API.Models;
namespace AppForSEII2526.API.DTOs.PedidoBocaDTOs
{
    public class CreatePedidoDTO
    {
        //PASO 5. El sistema muestra la lista de bocadillos seleccionados incluyendo su nombre, precio 
        //y tipo de pan
        //
        //
        //solicita la cantidad a comprar de cada bocadillo, siendo este campo
        //obligatorio.

        //Además, le pide al cliente que introduzca su nombre, apellidos y método
        //de pago (tarjeta de crédito o metálico), siendo todos los campos obligatorios, 
        //exceptuando el segundo apellido que será opcional(clientes extranjeros).

        

        public CreatePedidoDTO( string nombreCliente, string apellidoCliente1, string? apellidosCliente2, string metodo, IList<ItemPedidoDTO> bocadilloItem)
        {  
           
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente1 = apellidoCliente1 ?? throw new ArgumentNullException(nameof(apellidoCliente1)); ;
            ApellidoCliente2 = apellidosCliente2;
            Metodo = metodo;
            BocadilloItem = bocadilloItem;
        }
        public CreatePedidoDTO()
        {
            BocadilloItem = new List<ItemPedidoDTO>();
        }
       
        [Required]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 caracteres")]
        public string NombreCliente { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 caracteres")]
        public string ApellidoCliente1 { get; set; }

        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 caracteres")]
        public string? ApellidoCliente2 { get; set; }

       [Required]
        public string Metodo { get; set; } //************antes esto era objeto tmb en constructor
                                           

        public IList<ItemPedidoDTO> BocadilloItem { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CreatePedidoDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente1 == dTO.ApellidoCliente1 &&
                   ApellidoCliente2 == dTO.ApellidoCliente2 &&
                   Metodo == dTO.Metodo &&
                   EqualityComparer<IList<ItemPedidoDTO>>.Default.Equals(BocadilloItem, dTO.BocadilloItem);
        }
    }
}
