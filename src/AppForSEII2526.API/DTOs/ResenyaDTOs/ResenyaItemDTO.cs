using System;

namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaItemDTO
    {
        public ResenyaItemDTO(int bocadilloId, int puntuacion)
        {
            BocadilloId = bocadilloId;
            Puntuacion = puntuacion;
        }

        public ResenyaItemDTO() { }

        public int BocadilloId { get; set; }
        public int Puntuacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaItemDTO dto &&
                   BocadilloId == dto.BocadilloId &&
                   Puntuacion == dto.Puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Puntuacion);
        }
    }
}

