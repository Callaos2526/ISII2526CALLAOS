namespace AppForSEII2526.API.DTOs.ComprarMerch
{
    public class ComprarMerchandaisingDTO
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal PVP { get; set; }
        public int Stock { get; set; }
        public string Tipo { get; set; }
        public ComprarMerchandaisingDTO(int productoId, string nombre, decimal pvp, int stock, string tipo)
        {
            ProductoId = productoId;
            Nombre = nombre;
            PVP = pvp;
            Stock = stock;
            Tipo = tipo;
        }
    }
}
