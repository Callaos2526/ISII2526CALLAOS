namespace AppForSEII2526.API.Models
{
    public class Producto
    {
        public Producto() { }
        public Producto(int productoid, string nombreProducto, double pvp, int stock)
        {
            Productoid = productoid;
            NombreProducto = nombreProducto;
            PVP = pvp;
            Stock = stock;
        }

        [Key]
        public int Productoid { get; set; }
        public string NombreProducto { get; set; }
        public double PVP { get; set; }
        public int Stock { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public IList<Producto_Compra> producto_Compras { get; set; } = new List<Producto_Compra>();
        public override bool Equals(object? obj)
        {
            return obj is Producto producto &&
                   Productoid == producto.Productoid &&
                   NombreProducto == producto.NombreProducto &&
                   PVP == producto.PVP &&
                   Stock == producto.Stock;

        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Productoid, NombreProducto, PVP, Stock);
        }
    }
}
