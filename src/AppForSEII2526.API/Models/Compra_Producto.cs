namespace AppForSEII2526.API.Models
{
    public class Compra_Producto
    {
        public Compra_Producto(string apellido_1, string apellido_2, int compraid, string direccionEnvio, DateTime fechaCompra, string metodo_Pago, string nombre, int precioFinal) 
        {
            Apellido_1 = apellido_1;
            Apellido_2 = apellido_2;
            Compraid = compraid;
            DireccionEnvio = direccionEnvio;
            FechaCompra = fechaCompra;
            Metodo_Pago = metodo_Pago;
            Nombre = nombre;
            PrecioFinal = precioFinal;
        }
        [Key] // Foreing key?
        public int Compraid { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string DireccionEnvio { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; }
        [Required]
        public string Metodo_Pago { get; set; }
        [Required]
        public int PrecioFinal { get; set; }
        public string Apellido_1 { get; set; }
        public string Apellido_2 { get; set; }
        IList<Producto_Compra> ListaCompra = new List<Producto_Compra>();
        public override bool Equals(object? obj)
        {
            return obj is Compra_Producto compra_producto &&
                   Compraid == compra_producto.Compraid &&
                   Nombre == compra_producto.Nombre &&
                   DireccionEnvio == compra_producto.DireccionEnvio &&
                   FechaCompra == compra_producto.FechaCompra &&
                   Metodo_Pago == compra_producto.Metodo_Pago &&
                   PrecioFinal == compra_producto.PrecioFinal &&
                   Apellido_1 == compra_producto.Apellido_1 &&
                   Apellido_2 == compra_producto.Apellido_2;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Compraid, Nombre, DireccionEnvio, FechaCompra, Metodo_Pago, PrecioFinal, Apellido_1, Apellido_2);
        }
    }
}
