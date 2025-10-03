
namespace AppForSEII2526.API.Models
{
    public class TipoProducto
    {
        public TipoProducto(string nombre, int productoid)
        {
            Nombre = nombre;
            Productoid = productoid;
        }
        [Key]
        public int Productoid { get; set; }
        [Required]
        public string Nombre { get; set; }
        IList<Producto> Productos = new List<Producto>();
        public override bool Equals(object? obj)
        {
            return obj is TipoProducto producto &&
                   Productoid == producto.Productoid &&
                   Nombre == producto.Nombre;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Productoid, Nombre);
        }
    }

}
