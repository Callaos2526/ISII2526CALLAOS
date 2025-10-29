using AppForSEII2526.API.Models;
using Microsoft.CodeAnalysis;

namespace AppForSEII2526.API.DTOs.ComprarMerch
{
    public class ComprarMerchCreateDTO
    {
        //Paso 5. El sistema muestra al cliente un listado con todos los artículos incluyendo su nombre,
        //precio, tipo.También, el sistema muestra al cliente el formulario que tiene que rellenar
        //con nombre, apellidos, dirección de envío y método de pago(tarjeta de crédito, PayPal
        //o Google pay). Siendo necesario rellenar todos los campos para poder continuar, salvo
        //el campo segundo apellido, ya que es opcional.Para cada producto, se pide indicar la
        //cantidad que se va a comprar.
        public ComprarMerchCreateDTO(string nombre, string apellido_1, string? apellido_2, string direccionEnvio, MetodoPago metodo_Pago, int cantidad, IList<ComprarMerchItemDTO> merchItems)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellido_1 = apellido_1 ?? throw new ArgumentNullException(nameof(apellido_1));
            Apellido_2 = apellido_2; 
            Direccion_Envio = direccionEnvio ?? throw new ArgumentNullException(nameof(direccionEnvio));
            Metodo_Pago = metodo_Pago;
            Cantidad = cantidad;
            MerchItems = merchItems;
        }
        public ComprarMerchCreateDTO()
        {
                       MerchItems = new List<ComprarMerchItemDTO>();
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu nombre")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
        public string Nombre { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu primer apellido")]
        [StringLength(50, ErrorMessage = "El primer apellido no puede exceder 50 caracteres")]
        public string Apellido_1 { get; set; }

        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres")]
        public string? Apellido_2 { get; set; }

        [Required]
        public MetodoPago Metodo_Pago { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu dirección de envío")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 200 caracteres")]
        public string Direccion_Envio { get; set; }
        [Required]
        public int Cantidad { get; set; }
        public IList<ComprarMerchItemDTO> MerchItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComprarMerchCreateDTO dTO &&
                   Nombre == dTO.Nombre &&
                   Apellido_1 == dTO.Apellido_1 &&
                   Apellido_2 == dTO.Apellido_2 &&
                   EqualityComparer<MetodoPago>.Default.Equals(Metodo_Pago, dTO.Metodo_Pago) &&
                   Direccion_Envio == dTO.Direccion_Envio &&
                   Cantidad == dTO.Cantidad &&
                   EqualityComparer<IList<ComprarMerchItemDTO>>.Default.Equals(MerchItems, dTO.MerchItems);
        }
    }
}


