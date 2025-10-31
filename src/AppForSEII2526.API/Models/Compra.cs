namespace AppForSEII2526.API.Models
{
    public class Compra //en esta clase preguntar a noelia si tengo que quitar la inicializacion de los string del usuario abajo
    {
        public Compra() { } //si me pide que añada direccion la gestiono con ns q
        public Compra(int compraid, DateTime fechacompra, int nbocadillos, float preciototal, ApplicationUser applicationUser)
        {
            CompraId = compraid;
            FechaCompra = fechacompra;
            nBoadillos = nbocadillos;
            PrecioTotal = preciototal;
            ApplicationUser= applicationUser;
        }
        [Key]
        public int CompraId { get; set; }
        //[Required]
        //public string NombreCliente { get; set; }
        //[Required]
        //public string ApellidoCliente1 { get; set; }
        //public string? ApellidoCliente2 { get; set; }

        [Required]
        //Darle formato para que solo entre dia mes y año
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de compra")]
        public DateTime FechaCompra { get; set; }
        
        public int nBoadillos { get; set; }
        [Required]
        public MetodoPago metodoPago { get; set; }
        
        public float PrecioTotal { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        //Vector de relacion 1:N con CompraBocadillo
        
        public IList<CompraBocadillo> BocadillosComprados { get; set; } = new List<CompraBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Compra compra &&
                   CompraId == compra.CompraId &&
                   FechaCompra == compra.FechaCompra &&
                   nBoadillos == compra.nBoadillos &&
                   EqualityComparer<MetodoPago>.Default.Equals(metodoPago, compra.metodoPago) &&
                   PrecioTotal == compra.PrecioTotal &&
                   EqualityComparer<ApplicationUser>.Default.Equals(ApplicationUser, compra.ApplicationUser) &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(BocadillosComprados, compra.BocadillosComprados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CompraId, FechaCompra, nBoadillos, metodoPago, PrecioTotal, ApplicationUser, BocadillosComprados);
        }
    }
}
