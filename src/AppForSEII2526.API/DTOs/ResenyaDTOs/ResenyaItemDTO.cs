using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaItemDTO
    {
        // Constructor para el GET (detalle)
        public ResenyaItemDTO(int bocadilloId, string nombre, float pvp, string tamano, int puntuacion)
        {
            BocadilloId = bocadilloId;
            Nombre = nombre;
            Pvp = pvp;
            Tamano = tamano;
            Puntuacion = puntuacion;
        }

        // Constructor para el POST (crear reseña)
        public ResenyaItemDTO(int bocadilloId, int puntuacion)
        {
            BocadilloId = bocadilloId;
            Puntuacion = puntuacion;
        }

        public ResenyaItemDTO() { }

        [Required]
        public int BocadilloId { get; set; }

        // Estos tres son opcionales: solo se devuelven en el GET
        public string? Nombre { get; set; }
        public float? Pvp { get; set; }
        public string? Tamano { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "La puntuación debe estar entre 1 y 10.")]
        public int Puntuacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaItemDTO dTO &&
                   BocadilloId == dTO.BocadilloId &&
                   Nombre == dTO.Nombre &&
                   Pvp == dTO.Pvp &&
                   Tamano == dTO.Tamano &&
                   Puntuacion == dTO.Puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Nombre, Pvp, Tamano, Puntuacion);
        }
    }
}
