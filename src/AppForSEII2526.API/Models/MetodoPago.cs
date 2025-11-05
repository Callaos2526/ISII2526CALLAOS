namespace AppForSEII2526.API.Models
{
    public abstract class MetodoPago
    {
        public string metodoName { get; set; }
        [Key]
        public int metodoPagoId { get; set; }

        public IList<CompraBono> compraBonos { get; set; } = new List<CompraBono>();

    }

}
public class GooglePay : MetodoPago
{
    public GooglePay()
    {
        metodoName = "GooglePay";
    }

}
public class Paypal : MetodoPago
{
    public Paypal()
    {
        metodoName = "Paypal";
    }
}
public class Tarjeta : MetodoPago
{
    public Tarjeta()
    {
        metodoName = "Tarjeta";
    }


}

