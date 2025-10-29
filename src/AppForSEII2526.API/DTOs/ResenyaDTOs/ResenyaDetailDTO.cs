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


    }
}
