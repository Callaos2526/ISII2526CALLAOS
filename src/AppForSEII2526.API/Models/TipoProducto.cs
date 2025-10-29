
namespace AppForSEII2526.API.Models
{
    public class TipoProducto
    {
        public TipoProducto() { }
        public TipoProducto(string nombreProducto, int productoid)
        {
            NombreProducto = nombreProducto;
            Productoid = productoid;
        }
        [Key]
        public int Productoid { get; set; }
        public string NombreProducto { get; set; }
        public IList<Producto> Productos = new List<Producto>();
        public override bool Equals(object? obj)
        {
            return obj is TipoProducto producto &&
                   Productoid == producto.Productoid &&
                   NombreProducto == producto.NombreProducto;
        }

        
    }

}
