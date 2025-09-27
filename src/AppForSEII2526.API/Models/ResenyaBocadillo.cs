
namespace AppForSEII2526.API.Models
{
    public class ResenyaBocadillo
    {
        public ResenyaBocadillo(int bocadilloId, int puntuacion, string resenyaId)
        {
            BocadilloId = bocadilloId;
            Puntuacion = puntuacion;
            ResenyaId = resenyaId;
        }

        [Key]
        public int BocadilloId { get; set; }
        [Required]
        public int Puntuacion { get; set; }
        [Required]
        public string ResenyaId { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   Puntuacion == bocadillo.Puntuacion &&
                   ResenyaId == bocadillo.ResenyaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Puntuacion, ResenyaId);
        }
    }
}