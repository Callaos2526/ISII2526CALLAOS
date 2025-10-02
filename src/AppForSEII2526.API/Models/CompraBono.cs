namespace AppForSEII2526.API.Models
{
    public class CompraBono
    {
        // Constructor vacío

        public CompraBono()
        {
        }
        // Constructor de la clase CompraBono con atributos: ApellidoBono1, ApellidoBono2, CompraBonoId, FechaCompraBono, metodoPago, nBonos, NombreCliente, PrecioTotalBono
        public CompraBono(int compraBonoId, string nombreCliente, string apellidoBono1, string apellidoBono2, DateTime fechaCompraBono, PaymentMethodTypes metodoPago, int nBonos, double precioTotalBono)
        {
            CompraBonoId = compraBonoId;
            NombreCliente = nombreCliente;
            ApellidoBono1 = apellidoBono1;
            ApellidoBono2 = apellidoBono2;
            FechaCompraBono = fechaCompraBono;
            MetodoPago = metodoPago;
            NBonos = nBonos;
            PrecioTotalBono = precioTotalBono;
        }
        
        [Key]
        public int CompraBonoId { get; set; }
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string ApellidoBono1 { get; set; }
        [Required]
        public string ApellidoBono2 { get; set; }
        [Required]
        public DateTime FechaCompraBono { get; set; }
        [Required]
        public PaymentMethodTypes MetodoPago { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El número de bonos debe ser mayor que 0")]
        public int NBonos { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio total debe ser mayor que 0")]
        public double PrecioTotalBono { get; set; }
        [Required]
        public IList<BonosComprados> bonosComprados { get; set; } = new List<BonosComprados>();
        public enum PaymentMethodTypes
        {
            CreditCard,
            Cash,
            PayPal
        }


        public override bool Equals(object? obj)
        {
            return obj is CompraBono bono &&
                   CompraBonoId == bono.CompraBonoId &&
                   NombreCliente == bono.NombreCliente &&
                   ApellidoBono1 == bono.ApellidoBono1 &&
                   ApellidoBono2 == bono.ApellidoBono2 &&
                   FechaCompraBono == bono.FechaCompraBono &&
                   MetodoPago == bono.MetodoPago &&
                   NBonos == bono.NBonos &&
                   PrecioTotalBono == bono.PrecioTotalBono &&
                   EqualityComparer<IList<BonosComprados>>.Default.Equals(bonosComprados, bono.bonosComprados);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(CompraBonoId);
            hash.Add(NombreCliente);
            hash.Add(ApellidoBono1);
            hash.Add(ApellidoBono2);
            hash.Add(FechaCompraBono);
            hash.Add(MetodoPago);
            hash.Add(NBonos);
            hash.Add(PrecioTotalBono);
            hash.Add(bonosComprados);
            return hash.ToHashCode();
        }
    }
}
