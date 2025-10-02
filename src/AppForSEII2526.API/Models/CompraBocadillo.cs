
namespace AppForSEII2526.API.Models
{
    public class CompraBocadillo
    {
        public CompraBocadillo() { }
        public CompraBocadillo(int bocadilloid,int compraid, int cantidad, string nombrebocadillo, float precio, TipoPan tipopan)
        {   
            //clave foranea
            BocadilloId = bocadilloid;

            //clave foranea
            CompraId = compraid;

            Cantidad = cantidad;
            NombreBocadillo = nombrebocadillo;
            Precio = precio;
            TipoPan = tipopan;

        }
        //relacion con Bocadillo
        [Required]
        public int BocadilloId { get; set; } //clave foranea DE BOCADILLO
        

        //relacion con Compra
        [Required]
        public int CompraId { get; set; } //clave foranea DE COMPRA
       
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public string NombreBocadillo { get; set; }
        [Required]
        public float Precio { get; set; }
        [Required]
        public TipoPan TipoPan { get; set; }
        [Required]
        public Compra Compra { get; set; }
        [Required]
        public Bocadillo Bocadillo { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   CompraId == bocadillo.CompraId &&
                   Cantidad == bocadillo.Cantidad &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio &&
                   EqualityComparer<TipoPan>.Default.Equals(TipoPan, bocadillo.TipoPan) &&
                   EqualityComparer<Compra>.Default.Equals(Compra, bocadillo.Compra) &&
                   EqualityComparer<Bocadillo>.Default.Equals(Bocadillo, bocadillo.Bocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, CompraId, Cantidad, NombreBocadillo, Precio, TipoPan, Compra, Bocadillo);
        }
    }
}
