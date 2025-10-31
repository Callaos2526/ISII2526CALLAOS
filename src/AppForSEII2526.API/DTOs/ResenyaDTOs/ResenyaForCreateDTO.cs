
using static AppForSEII2526.API.Models.Resenya; 

namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaForCreateDTO
    {
        
        public ResenyaForCreateDTO(string? nombreUsuario,
                                   string titulo,
                                   string descripcion,
                                   ValoracionGeneral valoracion,
                                   IList<ResenyaItemDTO> resenyaBocadillo)
        {
            NombreUsuario = nombreUsuario;
            Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Valoracion = valoracion;
            ResenyaBocadillo = resenyaBocadillo ?? throw new ArgumentNullException(nameof(resenyaBocadillo));
        }

        
        public ResenyaForCreateDTO()
        {
            ResenyaBocadillo = new List<ResenyaItemDTO>();
        }

        [StringLength(100, ErrorMessage = "El nombre de usuario no puede superar 100 caracteres")]
        public string? NombreUsuario { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, escribe un título para la reseña")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 120 caracteres")]
        public string Titulo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, escribe una descripción para la reseña")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 2000 caracteres")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Por favor, selecciona una valoración general entre 1 y 5")]
        public ValoracionGeneral Valoracion { get; set; }

        [Required(ErrorMessage = "Debes incluir al menos un bocadillo con su puntuación")]
        [MinLength(1, ErrorMessage = "Debes puntuar al menos un bocadillo")]
        public IList<ResenyaItemDTO> ResenyaBocadillo { get; set; }

        [Display(Name = "Número de bocadillos puntuados")]
        [JsonPropertyName("NumeroDeBocadillos")]
        public int NumeroDeBocadillos => ResenyaBocadillo?.Count ?? 0;

        

        public override bool Equals(object? obj)
        {
            return obj is ResenyaForCreateDTO dTO &&
                   NombreUsuario == dTO.NombreUsuario &&
                   Titulo == dTO.Titulo &&
                   Descripcion == dTO.Descripcion &&
                   Valoracion == dTO.Valoracion &&
                   EqualityComparer<IList<ResenyaItemDTO>>.Default.Equals(ResenyaBocadillo, dTO.ResenyaBocadillo) &&
                   NumeroDeBocadillos == dTO.NumeroDeBocadillos;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreUsuario,Titulo, Descripcion, Valoracion, ResenyaBocadillo, NumeroDeBocadillos);
        }
    }
}
