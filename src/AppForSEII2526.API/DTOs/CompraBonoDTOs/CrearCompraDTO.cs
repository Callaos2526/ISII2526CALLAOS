using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class CrearCompraDTO
    {
        public CrearCompraDTO() { }

        public CrearCompraDTO(int compraId, string nombreCliente, string apellidoCliente1, string apellidoCliente2, DateTime fechaCompra, string metodoPagoName, IList<BonoItemForCreateDTO> bonoItem)
        {
            CompraId = compraId;
            NombreCliente = nombreCliente;
            ApellidoCliente1 = apellidoCliente1;
            ApellidoCliente2 = apellidoCliente2;
            MetodoPagoName = metodoPagoName;
            FechaCompra = fechaCompra;
            BonoItem = bonoItem;
        }

        [Key]
        public int CompraId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede ser más largo a 50 caracteres")]
        public string NombreCliente { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El apellido no puede ser más largo a 50 caracteres")]
        public string ApellidoCliente1 { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El apellido no puede ser más largo a 50 caracteres")]
        public string ApellidoCliente2 { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        // Ahora enviamos el nombre del método de pago (p. ej. "Tarjeta", "Paypal", "GooglePay")
        [Required]
        public string MetodoPagoName { get; set; }

        // Lista plana de items con id y cantidad
        [Required]
        public IList<BonoItemForCreateDTO> BonoItem { get; set; }
    }
}
