namespace AppForSEII2526.API.Models
{
    public abstract class MetodoPago
    {
        public string metodoName { get; set; }
        [Key]
        public int metodoPagoId { get; set; }

        public IList<Compra> m { get; set; } = new List<Compra>();

    }
}
