using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.ComprarMerch
{
    //Paso 7. El sistema muestra una confirmación y un ticket indicando todos los datos del cliente
    //(nombre, apellidos, dirección y método de pago) y todos los artículos que ha comprado
    //(cada uno con su nombre, tipo, precio y cantidad).
    public class ComprarMerchDetailDTO : ComprarMerchCreateDTO
    {
        public ComprarMerchDetailDTO(int id, int compraId,ApplicationUser cliente, string direccionEnvio, string metodoPago,int cantidad, IList<ComprarMerchItemDTO> merchItems)
            : base(compraId,cliente.NombreCliente, cliente.ApellidoCliente1, cliente.ApellidoCliente2, direccionEnvio, metodoPago, cantidad, merchItems)
        {
            Id = id;
        }

        public int Id { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComprarMerchDetailDTO dTO &&
                   base.Equals(obj) &&
                   CompraId == dTO.CompraId &&
                   Nombre == dTO.Nombre &&
                   Apellido_1 == dTO.Apellido_1 &&
                   Apellido_2 == dTO.Apellido_2 &&
                   Metodo_Pago == dTO.Metodo_Pago &&
                   Direccion_Envio == dTO.Direccion_Envio &&
                   Cantidad == dTO.Cantidad &&
                   EqualityComparer<IList<ComprarMerchItemDTO>>.Default.Equals(MerchItems, dTO.MerchItems) &&
                   Id == dTO.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Apellido_1, Apellido_2, Metodo_Pago, Direccion_Envio, Cantidad, MerchItems, Id);
        }
    }

}
