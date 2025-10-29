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
                                ValoracionGeneral valoracion,
                                IList<ResenyaItemDTO> resenyaBocadillo)
        {
            Id = id;
            FechaPublicacion = fechaPublicacion;
            NombreUsuario = nombreUsuario;
            Titulo = titulo;
            Descripcion = descripcion;
            Valoracion = valoracion;
            ResenyaBocadillo = resenyaBocadillo ?? new List<ResenyaItemDTO>();
        }

        public ResenyaDetailDTO()
        {
            ResenyaBocadillo = new List<ResenyaItemDTO>();
        }


        public int Id { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string? NombreUsuario { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public ValoracionGeneral Valoracion { get; set; }
        public IList<ResenyaItemDTO> ResenyaBocadillo { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is ResenyaDetailDTO dto &&
                   Id == dto.Id &&
                   FechaPublicacion == dto.FechaPublicacion &&
                   NombreUsuario == dto.NombreUsuario &&
                   Titulo == dto.Titulo &&
                   Descripcion == dto.Descripcion &&
                   Valoracion == dto.Valoracion &&
                   EqualityComparer<IList<ResenyaItemDTO>>.Default.Equals(ResenyaBocadillo, dto.ResenyaBocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FechaPublicacion, NombreUsuario, Titulo, Descripcion, Valoracion);
        }
    }
}
