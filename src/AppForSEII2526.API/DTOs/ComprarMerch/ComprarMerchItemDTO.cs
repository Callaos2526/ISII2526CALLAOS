
namespace AppForSEII2526.API.DTOs.ComprarMerch
{
    public class ComprarMerchItemDTO
    {
        //Paso 5 El sistema muestra al cliente un listado con todos los artículos incluyendo su nombre,
        //precio, tipo.
        public ComprarMerchItemDTO(int id, string nombreProducto, double pvp, string tipo, int cantidad)
        {
            Id = id;
            NombreProducto = nombreProducto;
            PVP = pvp;
            Tipo = tipo;
            Cantidad = cantidad;
        }
        [Key]
        public int Id { get; set; }
        public string NombreProducto { get; set; }
        public double PVP { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComprarMerchItemDTO dTO &&
                   Id == dTO.Id &&
                   NombreProducto == dTO.NombreProducto &&
                   PVP == dTO.PVP &&
                   Tipo == dTO.Tipo &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, NombreProducto, PVP, Tipo);
        }
    }
}
