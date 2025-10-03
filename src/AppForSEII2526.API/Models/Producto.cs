namespace AppForSEII2526.API.Models
{
    public class Producto
    {
        public Producto() { }
        public Producto(int productoid, string nombre, int pvp, int stock)
        {
            Productoid = productoid;
            Nombre = nombre;
            PVP = pvp;
            Stock = stock;
        }

        [Key]
        public int Productoid { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int PVP { get; set; }
        [Required]
        public int Stock { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public IList<Producto_Compra> producto_Compras = new List<Producto_Compra>();
        public override bool Equals(object? obj)
        {
            return obj is Producto producto &&
                   Productoid == producto.Productoid &&
                   Nombre == producto.Nombre &&
                   PVP == producto.PVP &&
                   Stock == producto.Stock;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Productoid, Nombre, PVP, Stock);
        }
    }
}
