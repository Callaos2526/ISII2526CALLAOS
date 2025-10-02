
namespace AppForSEII2526.API.Models
{
    public class TipoPan
    {
        //constructor vacio
        public TipoPan() { }
        //constructor con parametros
        public TipoPan(int panid, string nombre)
        {
            PanId = panid;
            Nombre = nombre;
        }

        [Key]
        public int PanId { get; set; }
        [Required]
        public required string Nombre { get; set; } //obliga a que no sea null aunque uses constructor vacio
        //Vector de relacion 1:N con Bocadillo
        [Required]
        public IList<Bocadillo> Bocadillos { get; set; }=new List<Bocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is TipoPan pan &&
                   PanId == pan.PanId &&
                   Nombre == pan.Nombre &&
                   EqualityComparer<IList<Bocadillo>>.Default.Equals(Bocadillos, pan.Bocadillos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PanId, Nombre, Bocadillos);
        }

      
    }
}
