using static AppForSEII2526.API.Models.Resenya;

namespace AppForSEII2526.API.Models
{
    public class ResenyaBocadillo
    {
        public ResenyaBocadillo() { }
        public ResenyaBocadillo(int id, int bocadilloId, int puntuacion, int resenyaId)
        {
            Id = id; 
            BocadilloId = bocadilloId;
            Puntuacion = puntuacion;
            ResenyaId = resenyaId;
        }

        [Key]
        public int Id { get; set; }
        public int BocadilloId { get; set; }
        [Required]
        public int Puntuacion { get; set; }
        public int ResenyaId { get; set; }

        [Required]
        public Resenya Resenya { get; set; }
        [Required]
        public Bocadillo Bocadillo { get; set; }
        

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