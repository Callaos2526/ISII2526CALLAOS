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

