namespace AppForSEII2526.API.Models
{
    public class Resenya
    {
        public Resenya() { }
        public Resenya(int id, string descripcion, DateTime fechaPublicacion, string nombreUsuario, string titulo)
        {
            Id = id;
            Descripcion = descripcion;
            FechaPublicacion = fechaPublicacion;
            NombreUsuario = nombreUsuario;
            Titulo = titulo;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string? NombreUsuario { get; set; }
        [Required]
        public string Titulo { get; set; }

        [Required]
        public ValoracionGeneral Valoracion { get; set; }


        public IList<ResenyaBocadillo> ResenyaBocadillo { get; set; } = new List<ResenyaBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Resenya resenya &&
                   Id == resenya.Id &&
                   Descripcion == resenya.Descripcion &&
                   FechaPublicacion == resenya.FechaPublicacion &&
                   NombreUsuario == resenya.NombreUsuario &&
                   Titulo == resenya.Titulo &&
                   EqualityComparer<IList<ResenyaBocadillo>>.Default.Equals(ResenyaBocadillo, resenya.ResenyaBocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Descripcion, FechaPublicacion, NombreUsuario, Titulo, ResenyaBocadillo);
        }

        public enum ValoracionGeneral
        {
            Uno,
            Dos,
            Tres,
            Cuatro,
            Cinco
        }
    }
}