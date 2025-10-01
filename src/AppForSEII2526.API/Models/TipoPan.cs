
namespace AppForSEII2526.API.Models
{
    public class TipoPan
    {
        public TipoPan(string nombre,int panId)
        {
            PanId = panId;
            Nombre = nombre;
        }
        [Key]
        public int PanId { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public List<Bocadillo> Bocadillos { get; set; } = new List<Bocadillo>();


        public override bool Equals(object? obj)
        {
            return obj is TipoPan pan &&
                   PanId == pan.PanId &&
                   Nombre == pan.Nombre;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PanId, Nombre);
        }
    }
}
