using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace AppForSEII2526.API.Models
{
    public abstract class MetodoPago
    {
        public MetodoPago() { }
        public string metodoName { get; set; }
        [Key]
        public int metodoPagoId { get; set; }

        public IList<CompraBono> compraBonos { get; set; } = new List<CompraBono>();

    }
        public class MetodoPagoTarjeta : MetodoPago
        {
            public MetodoPagoTarjeta()
            {
                metodoName = "Tarjeta";
            }
        }

        public class MetodoPagoPayPal : MetodoPago
        {
            public MetodoPagoPayPal()
            {
                metodoName = "Paypal";
            }
        }

        public class MetodoPagoGooglePay : MetodoPago
        {
            public MetodoPagoGooglePay()
            {
                metodoName = "GooglePay";
            }
        }
}



