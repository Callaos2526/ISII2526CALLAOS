using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class CompraBonoDetallesDTO : CrearCompraDTO
    {
        //7. El sistema muestra los datos de la compra realizada, mostrando el nombre y apellidos al que están los bonos adquiridos,
        //el método de pago y la fecha en la que se hizo, así como el precio total. De cada bono se muestra el nombre, tipo, precio y cantidad.
        public CompraBonoDetallesDTO(int id, ApplicationUser Cliente, MetodoPago metodo, DateTime fechaCompra,
                                        double precioTotal, IList<BonoItemForCreateDTO> bonoItem)
            : base(id, Cliente.NombreCliente, Cliente.ApellidoCliente1, Cliente.ApellidoCliente2, fechaCompra, metodo.metodoName, bonoItem)
        {
            ID = id;
            PrecioTotal = precioTotal;
        }
        [Key]
        public int ID { get; set; }
        public double PrecioTotal { get; set; }
    }
}
