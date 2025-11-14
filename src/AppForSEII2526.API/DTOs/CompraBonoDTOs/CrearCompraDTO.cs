using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class CrearCompraDTO
    {
        public CrearCompraDTO() 
        { 
            NombreCliente = string.Empty;
            ApellidoCliente1 = string.Empty;
            ApellidoCliente2 = string.Empty;
            MetodoPagoName = string.Empty;
            BonoItem = new List<BonoItemForCreateDTO>();
        }

        public CrearCompraDTO(int compraId, string nombreCliente, string apellidoCliente1, string apellidoCliente2, DateTime fechaCompra, string metodoPagoName, IList<BonoItemForCreateDTO> bonoItem)
        {
            CompraId = compraId;
            NombreCliente = nombreCliente ?? string.Empty;
            ApellidoCliente1 = apellidoCliente1 ?? string.Empty;
            ApellidoCliente2 = apellidoCliente2 ?? string.Empty;
            MetodoPagoName = metodoPagoName ?? string.Empty;
            FechaCompra = fechaCompra;
            BonoItem = bonoItem ?? new List<BonoItemForCreateDTO>();
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

        public override bool Equals(object? obj)
        {
            if (obj is not CrearCompraDTO dTO) return false;

            return CompraId == dTO.CompraId &&
                   string.Equals(NombreCliente, dTO.NombreCliente, StringComparison.Ordinal) &&
                   string.Equals(ApellidoCliente1, dTO.ApellidoCliente1, StringComparison.Ordinal) &&
                   string.Equals(ApellidoCliente2, dTO.ApellidoCliente2, StringComparison.Ordinal) &&
                   FechaCompra.Date == dTO.FechaCompra.Date && // Solo comparar la fecha, no la hora
                   string.Equals(MetodoPagoName, dTO.MetodoPagoName, StringComparison.Ordinal) &&
                   BonoItemsEqual(BonoItem, dTO.BonoItem);
        }

        private bool BonoItemsEqual(IList<BonoItemForCreateDTO>? list1, IList<BonoItemForCreateDTO>? list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CompraId, NombreCliente, ApellidoCliente1, ApellidoCliente2, FechaCompra.Date, MetodoPagoName);
        }

        public ApplicationUser ToApplicationUser()
        {
            return new ApplicationUser(NombreCliente, ApellidoCliente1, ApellidoCliente2);
        }

        public Tarjeta ToTarjeta()
        {
            return new Tarjeta() { metodoName = MetodoPagoName };
        }
    }
}
