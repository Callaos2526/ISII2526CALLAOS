
namespace AppForSEII2526.API.Models
{
    public class BonosComprados
    {
        public BonosComprados(int bonoId, int cantidad, int compraId, double precioBono, TipoBocadillo tipo, BonosComprados bonos)
        {
            BonoId = bonoId;
            Cantidad = cantidad;
            CompraId = compraId;
            PrecioBono = precioBono;
            Tipo = tipo;
            Bonos = bonos;
        }
        [Key]
        public int BonoId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
        [Required]
        public int CompraId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public double PrecioBono { get; set; }
        [Required]
        public TipoBocadillo Tipo { get; set; }
        [Required]
        public BonosComprados Bonos { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BonosComprados comprados &&
                   BonoId == comprados.BonoId &&
                   Cantidad == comprados.Cantidad &&
                   CompraId == comprados.CompraId &&
                   PrecioBono == comprados.PrecioBono &&
                   EqualityComparer<TipoBocadillo>.Default.Equals(Tipo, comprados.Tipo) &&
                   EqualityComparer<BonosComprados>.Default.Equals(Bonos, comprados.Bonos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BonoId, Cantidad, CompraId, PrecioBono, Tipo, Bonos);
        }
    }
}
