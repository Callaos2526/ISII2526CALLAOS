
namespace AppForSEII2526.API.DTOs.ResenyaDTOs
{
    public class ResenyaItemDTO
    {
        public ResenyaItemDTO(int bocadilloId, string nombre, float pvp, string tamano)
        {
            BocadilloId = bocadilloId;
            Nombre = nombre;
            Pvp = pvp;
            Tamano = tamano;
        }

        public ResenyaItemDTO() { }

        public int BocadilloId { get; set; }
        public string Nombre { get; set; }
        public float Pvp { get; set; }
        public string Tamano { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaItemDTO dTO &&
                   BocadilloId == dTO.BocadilloId &&
                   Nombre == dTO.Nombre &&
                   Pvp == dTO.Pvp &&
                   Tamano == dTO.Tamano;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Nombre, Pvp, Tamano);
        }
    }
}

