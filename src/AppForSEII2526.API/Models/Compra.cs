namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra() { }
        public Compra(string apellido_1cliente, string apellido_2cliente, int compraid, DateTime fechacompra, int nbocadillos, string nombrecliente, float preciototal)
        {
            Apellido_1Cliente = apellido_1cliente;
            Apellido_2Cliente = apellido_2cliente;
            CompraId = compraid;
            FechaCompra = fechacompra;
            nBoadillos = nbocadillos;
            NombreCliente = nombrecliente;
            PrecioTotal = preciototal;
        }
        [Key]
        public int CompraId { get; set; }
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string Apellido_1Cliente { get; set; }
        [Required]
        public string Apellido_2Cliente { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; }
        [Required]
        public int nBoadillos { get; set; }
        [Required]
        public MetodoPago metodoPago { get; set; }
        [Required]
        public float PrecioTotal { get; set; }
        //Vector de relacion 1:N con CompraBocadillo
        [Required]
        public IList<CompraBocadillo> BocadillosComprados { get; set; } = new List<CompraBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Compra compra &&
                   CompraId == compra.CompraId &&
                   NombreCliente == compra.NombreCliente &&
                   Apellido_1Cliente == compra.Apellido_1Cliente &&
                   Apellido_2Cliente == compra.Apellido_2Cliente &&
                   FechaCompra == compra.FechaCompra &&
                   nBoadillos == compra.nBoadillos &&
                   PrecioTotal == compra.PrecioTotal &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(BocadillosComprados, compra.BocadillosComprados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CompraId, NombreCliente, Apellido_1Cliente, Apellido_2Cliente, FechaCompra, nBoadillos, PrecioTotal, BocadillosComprados);
        }

       

    }
}
