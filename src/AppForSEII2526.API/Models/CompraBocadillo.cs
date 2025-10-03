
namespace AppForSEII2526.API.Models
{
    public class CompraBocadillo
    {
        public CompraBocadillo() { }
        public CompraBocadillo(int id, int bocadilloid,int compraid, int cantidad, string nombrebocadillo, float precio)
        {   
            Id = id;
            //clave foranea
            BocadilloId = bocadilloid;

            //clave foranea
            CompraId = compraid;

            Cantidad = cantidad;
            NombreBocadillo = nombrebocadillo;
            Precio = precio;
        }
        [Key]
        public int Id { get; set; }
        [ForeignKey("BocadilloId")]
        //relacion con Bocadillo
        public int BocadilloId { get; set; } //clave foranea DE BOCADILLO
        
        [ForeignKey("CompraId")]
        //relacion con Compra
        public int CompraId { get; set; } //clave foranea DE COMPRA
       
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public string NombreBocadillo { get; set; }
        [Required]
        public float Precio { get; set; }
        [Required]
        public List<TipoPan> TipoPan { get; set; } = new List<TipoPan>();
        public Compra Compra { get; set; }
        public Bocadillo Bocadillo { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   CompraId == bocadillo.CompraId &&
                   Cantidad == bocadillo.Cantidad &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio &&
                   EqualityComparer<Compra>.Default.Equals(Compra, bocadillo.Compra) &&
                   EqualityComparer<Bocadillo>.Default.Equals(Bocadillo, bocadillo.Bocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, CompraId, Cantidad, NombreBocadillo, Precio, TipoPan, Compra, Bocadillo);
        }
    }
}
