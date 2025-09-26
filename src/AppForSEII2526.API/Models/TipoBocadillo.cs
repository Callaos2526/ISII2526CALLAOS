
namespace AppForSEII2526.API.Models
{
    public class TipoBocadillo
    {
        public TipoBocadillo(int panid, string nombreTipo)
        {
            PanId = panid;
            NombreTipo = nombreTipo;
        }

        [Key]
        public int PanId { get; set; }
        [Required]
        public string NombreTipo { get; set; }

        IList<TipoBocadillo> bocadillos = new List<TipoBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is TipoBocadillo bocadillo &&
                   PanId == bocadillo.PanId &&
                   NombreTipo == bocadillo.NombreTipo;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PanId, NombreTipo);
        }
    }
}
