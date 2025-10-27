using System.Runtime.CompilerServices;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class CompraBonoDetallesDTO : CrearCompraDTO
    {
        //7. El sistema muestra los datos de la compra realizada, mostrando el nombre y apellidos al que están los bonos adquiridos,
        //el método de pago y la fecha en la que se hizo, así como el precio total. De cada bono se muestra el nombre, tipo, precio y cantidad.
        public CompraBonoDetallesDTO(int id, ApplicationUser nombreCliente, ApplicationUser apellidoCliente1, ApplicationUser apellidoCliente2, MetodoPago metodo, DateTime fechaCompra,
                                        double precioTotal, IList<BonoItemDTO> bonoItem)
            : base(id, nombreCliente, apellidoCliente1, apellidoCliente2, fechaCompra, metodo, bonoItem)
        {
            ID = id;
            PrecioTotal = precioTotal;
        }
        [Key]
        public int ID { get; set; }
        public double PrecioTotal { get; set; }


    }
}
