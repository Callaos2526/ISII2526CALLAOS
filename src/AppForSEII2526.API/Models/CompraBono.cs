namespace AppForSEII2526.API.Models
{
    public class CompraBono
    {
        // Constructor vacío

        public CompraBono()
        {
        }
        // Constructor de la clase CompraBono con atributos: ApellidoBono1, ApellidoBono2, CompraBonoId, FechaCompraBono, metodoPago, nBonos, NombreCliente, PrecioTotalBono
        public CompraBono(int compraBonoId, ApplicationUser cliente, DateTime fechaCompraBono, MetodoPago metodoPago, int nBonos, double precioTotalBono, IList<BonosComprados> bonosComprados)
        {
            CompraBonoId = compraBonoId;
            FechaCompraBono = fechaCompraBono;
            MetodoPago = metodoPago;
            NBonos = nBonos;
            PrecioTotalBono = precioTotalBono;
            Cliente = cliente;
        }
        
        [Key]
        public int CompraBonoId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime FechaCompraBono { get; set; }
        [Required]
        public MetodoPago  MetodoPago { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El número de bonos debe ser mayor que 0")]
        public int NBonos { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio total debe ser mayor que 0")]
        public double PrecioTotalBono { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ApplicationUser Cliente { get; set; }

        public IList<BonosComprados> bonosComprados { get; set; } = new List<BonosComprados>();


        public override bool Equals(object? obj)
        {
            return obj is CompraBono bono &&
                   CompraBonoId == bono.CompraBonoId &&
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
            hash.Add(FechaCompraBono);
            hash.Add(MetodoPago);
            hash.Add(NBonos);
            hash.Add(PrecioTotalBono);
            hash.Add(bonosComprados);
            return hash.ToHashCode();
        }
    }
}
