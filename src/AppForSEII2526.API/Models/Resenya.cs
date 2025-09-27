namespace AppForSEII2526.API.Models
{
    public class Resenya
    {
        public Resenya(int id, string descripcion, DateTime fechaPublicacion, string nombreUsuario, string titulo)
        {
            Id = id;
            this.descripcion = descripcion;
            this.fechaPublicacion = fechaPublicacion;
            this.nombreUsuario = nombreUsuario;
            this.titulo = titulo;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public DateTime fechaPublicacion { get; set; }
        public string nombreUsuario { get; set; }
        [Required]
        public string titulo { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Resenya resenya &&
                   Id == resenya.Id &&
                   descripcion == resenya.descripcion &&
                   fechaPublicacion == resenya.fechaPublicacion &&
                   nombreUsuario == resenya.nombreUsuario &&
                   titulo == resenya.titulo;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, descripcion, fechaPublicacion, nombreUsuario, titulo);
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