namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaItemDTO
    {
        public ResenyaItemDTO(int bocadilloId, string nombre, float pvp, string tamano, string? tipoPanNombre, int puntuacion)
        {
            BocadilloId = bocadilloId;
            Nombre = nombre;
            Pvp = pvp;
            Tamano = tamano;
            TipoPanNombre = tipoPanNombre;
            Puntuacion = puntuacion;
        }

        public ResenyaItemDTO() { }

        public int BocadilloId { get; set; }
        public string Nombre { get; set; }
        public float Pvp { get; set; }
        public string Tamano { get; set; }
        public string? TipoPanNombre { get; set; }
        public int Puntuacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaItemDTO dto &&
                   BocadilloId == dto.BocadilloId &&
                   Nombre == dto.Nombre &&
                   Pvp == dto.Pvp &&
                   Tamano == dto.Tamano &&
                   TipoPanNombre == dto.TipoPanNombre &&
                   Puntuacion == dto.Puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Nombre, Pvp, Tamano, TipoPanNombre, Puntuacion);
        }
    }
}

