

namespace AppForSEII2526.API.Models
{
    public class TipoBocadillo
    {
        //Constructor vacío
        public TipoBocadillo()
        {
        }
        public TipoBocadillo(int idTipo, string nombreTipo)
        {
            IdTipo = idTipo;
            NombreTipo = nombreTipo;
        }


        [Key]
        public int IdTipo { get; set; }
        [Required]
        public string NombreTipo { get; set; }
        
        public IList<BonoBocadillo> Bonos { get; set ; } = new List<BonoBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is TipoBocadillo bocadillo &&
                   IdTipo == bocadillo.IdTipo &&
                   NombreTipo == bocadillo.NombreTipo &&
                   EqualityComparer<IList<BonoBocadillo>>.Default.Equals(Bonos, bocadillo.Bonos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdTipo, NombreTipo, Bonos);
        }
    }
}
