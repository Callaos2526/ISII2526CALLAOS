namespace AppForSEII2526.API.Models
{
    public class Compra_Producto
    {
        public Compra_Producto() { }
        public Compra_Producto( int compraid, ApplicationUser cliente, string direccionEnvio, DateTime fechaCompra, MetodoPago metodo_Pago, int precioFinal) 
        {
            Compraid = compraid;
            Cliente = cliente;
            DireccionEnvio = direccionEnvio;
            FechaCompra = fechaCompra;
            Metodo_Pago = metodo_Pago;
            PrecioFinal = precioFinal;   
        }
        [Key] 
        public int Compraid { get; set; }
        [Required]
        public ApplicationUser Cliente { get; set; }
        [Required]
        public string DireccionEnvio { get; set; }
        public DateTime FechaCompra { get; set; }
        [Required]
        public MetodoPago Metodo_Pago { get; set; }
        [Required]
        public int PrecioFinal { get; set; }
        [Required]
        public IList<Producto_Compra> ListaCompra = new List<Producto_Compra>();

        public override bool Equals(object? obj)
        {
            return obj is Compra_Producto producto &&
                   Compraid == producto.Compraid &&
                   DireccionEnvio == producto.DireccionEnvio &&
                   FechaCompra == producto.FechaCompra &&
                   EqualityComparer<MetodoPago>.Default.Equals(Metodo_Pago, producto.Metodo_Pago) &&
                   PrecioFinal == producto.PrecioFinal &&
                   EqualityComparer<IList<Producto_Compra>>.Default.Equals(ListaCompra, producto.ListaCompra);
        }
    }
}
