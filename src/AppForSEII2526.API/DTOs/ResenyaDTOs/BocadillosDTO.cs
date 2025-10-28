namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class BocadillosDTO
    {
        public string Nombre { get; set; }
        public float Pvp { get; set; }
        public string Tamano { get; set; }
        public string? TipoPanNombre { get; set; }
        public BocadillosDTO(string nombre, float pvp, string tamano, string? tipoPanNombre)
        {
            Nombre = nombre;
            Pvp = pvp;
            Tamano = tamano;
            TipoPanNombre = tipoPanNombre;
        }

        public BocadillosDTO() { }

        public override bool Equals(object? obj)
        {
            return obj is BocadillosDTO dto &&
                   Nombre == dto.Nombre &&
                   Pvp == dto.Pvp &&
                   Tamano == dto.Tamano &&
                   TipoPanNombre == dto.TipoPanNombre;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Pvp, Tamano, TipoPanNombre);
        }
    }
}
