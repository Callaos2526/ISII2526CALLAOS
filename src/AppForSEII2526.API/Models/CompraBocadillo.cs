
namespace AppForSEII2526.API.Models
{
    public class CompraBocadillo
    {
        public CompraBocadillo(int bocadilloId,int cantidad,int compraId,string nombreBocadillo,int precio,TipoPan tipopan)
        {
            BocadilloId = bocadilloId;
            Cantidad = cantidad;
            CompraId = compraId;
            NombreBocadillo = nombreBocadillo;
            Precio = precio;
            Tipopan = tipopan;
        }
        [Required]
        public int BocadilloId { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Key]
        public int CompraId { get; set; }
        [Required]
        public string NombreBocadillo { get; set; }
        [Required]
        public int Precio { get; set; }
        [Required]
        public TipoPan Tipopan { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   Cantidad == bocadillo.Cantidad &&
                   CompraId == bocadillo.CompraId &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio &&
                   EqualityComparer<TipoPan>.Default.Equals(Tipopan, bocadillo.Tipopan);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Cantidad, CompraId, NombreBocadillo, Precio, Tipopan);
        }
    }
}
