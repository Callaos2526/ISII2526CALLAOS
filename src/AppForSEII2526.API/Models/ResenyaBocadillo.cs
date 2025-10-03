using static AppForSEII2526.API.Models.Resenya;

namespace AppForSEII2526.API.Models
{
    public class ResenyaBocadillo
    {
        public ResenyaBocadillo(int bocadilloId, int puntuacion, int resenyaId)
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
        public int ResenyaId { get; set; }

        [Required]
        public Resenya Resenya { get; set; }
        public int Resenyaid { get; set; }
        [Required]
        public Bocadillo Bocadillo { get; set; }
        public int Bocadilloid { get; set; }
        

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