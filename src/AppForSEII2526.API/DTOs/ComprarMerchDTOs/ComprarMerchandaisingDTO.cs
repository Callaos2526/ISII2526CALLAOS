namespace AppForSEII2526.API.DTOs.ComprarMerch
{
    public class ComprarMerchandaisingDTO
    {
        //Paso 2. El sistema muestra un listado con todos los productos de merchandising disponibles
        //(que tengan stock) indicando su nombre, precio, tipo y el stock.
        public ComprarMerchandaisingDTO() { }
        public ComprarMerchandaisingDTO(int productoId, string nombreProducto, double pvp, int stock, string tipoProducto)
        {
            ProductoId = productoId;
            NombreProducto = nombreProducto;
            PVP = pvp;
            Stock = stock;
            TipoProducto = tipoProducto;
        }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public double PVP { get; set; }
        public int Stock { get; set; }
        public string TipoProducto { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComprarMerchandaisingDTO dTO &&
                   ProductoId == dTO.ProductoId &&
                   NombreProducto == dTO.NombreProducto &&
                   PVP == dTO.PVP &&
                   Stock == dTO.Stock &&
                   TipoProducto == dTO.TipoProducto;
        }
    }
}
