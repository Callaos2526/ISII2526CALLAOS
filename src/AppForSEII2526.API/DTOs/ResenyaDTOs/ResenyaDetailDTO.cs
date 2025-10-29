using static AppForSEII2526.API.Models.Resenya;

namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaDetailDTO : ResenyaForCreateDTO
    {
        public ResenyaDetailDTO(int id,
                                DateTime fechaPublicacion,
                                string? nombreUsuario,
                                string titulo,
                                string descripcion,
                                int puntuacion,
                                ValoracionGeneral valoracion,
                                IList<ResenyaItemDTO> resenyaBocadillo)
            : base(nombreUsuario, titulo, descripcion, valoracion, puntuacion, resenyaBocadillo)
        {
            Id = id;
            FechaPublicacion = fechaPublicacion;
           
        }


        public int Id { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de publicacion")]
        public DateTime FechaPublicacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaDetailDTO dTO &&
                   base.Equals(obj) &&
                   NombreUsuario == dTO.NombreUsuario &&
                   Puntuacion == dTO.Puntuacion &&
                   Titulo == dTO.Titulo &&
                   Descripcion == dTO.Descripcion &&
                   Valoracion == dTO.Valoracion &&
                   EqualityComparer<IList<ResenyaItemDTO>>.Default.Equals(ResenyaBocadillo, dTO.ResenyaBocadillo) &&
                   NumeroDeBocadillos == dTO.NumeroDeBocadillos &&
                   Id == dTO.Id &&
                   FechaPublicacion == dTO.FechaPublicacion;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(NombreUsuario);
            hash.Add(Puntuacion);
            hash.Add(Titulo);
            hash.Add(Descripcion);
            hash.Add(Valoracion);
            hash.Add(ResenyaBocadillo);
            hash.Add(NumeroDeBocadillos);
            hash.Add(Id);
            hash.Add(FechaPublicacion);
            return hash.ToHashCode();
        }
    }
}
