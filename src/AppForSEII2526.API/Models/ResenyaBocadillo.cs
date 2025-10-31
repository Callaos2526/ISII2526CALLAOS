using static AppForSEII2526.API.Models.Resenya;

namespace AppForSEII2526.API.Models
{
    public class ResenyaBocadillo
    {
        public ResenyaBocadillo() { }
        public ResenyaBocadillo(int id, int bocadilloId, int resenyaId,int puntuacion)
        {
            Id = id; 
            BocadilloId = bocadilloId;
            ResenyaId = resenyaId;
            Puntuacion = puntuacion;
        }

        [Key]
        public int Id { get; set; }
        public int BocadilloId { get; set; }
        public int ResenyaId { get; set; }

        public Resenya Resenya { get; set; }
        public Bocadillo Bocadillo { get; set; }

        public int Puntuacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaBocadillo bocadillo &&
                   Id == bocadillo.Id &&
                   BocadilloId == bocadillo.BocadilloId &&
                   ResenyaId == bocadillo.ResenyaId &&
                   EqualityComparer<Resenya>.Default.Equals(Resenya, bocadillo.Resenya) &&
                   EqualityComparer<Bocadillo>.Default.Equals(Bocadillo, bocadillo.Bocadillo) &&
                   Puntuacion == bocadillo.Puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BocadilloId, ResenyaId, Resenya, Bocadillo, Puntuacion);
        }
    }
}