namespace AppForSEII2526.API.Models
{
    public class Compra_Producto
    {
        public Compra_Producto() { }
        public Compra_Producto(string apellido_1,string? apellido_2, int compraid, string direccionEnvio, DateTime fechaCompra, MetodoPago metodo_Pago, string nombre, int precioFinal, ApplicationUser applicationUser) 
        {
            Apellido_1 = apellido_1;
            Apellido_2 = apellido_2;
            Compraid = compraid;
            DireccionEnvio = direccionEnvio;
            FechaCompra = fechaCompra;
            Metodo_Pago = metodo_Pago;
            Nombre = nombre;
            PrecioFinal = precioFinal;
            ApplicationUser = applicationUser;
        }
        [Key] 
        public int Compraid { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string DireccionEnvio { get; set; }
        public DateTime FechaCompra { get; set; }
        [Required]
        public MetodoPago Metodo_Pago { get; set; }
        [Required]
        public int PrecioFinal { get; set; }
        [Required]
        public string Apellido_1 { get; set; }
        public string? Apellido_2 { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IList<Producto_Compra> ListaCompra = new List<Producto_Compra>();

        public override bool Equals(object? obj)
        {
            return obj is Compra_Producto producto &&
                   Compraid == producto.Compraid &&
                   Nombre == producto.Nombre &&
                   DireccionEnvio == producto.DireccionEnvio &&
                   FechaCompra == producto.FechaCompra &&
                   EqualityComparer<MetodoPago>.Default.Equals(Metodo_Pago, producto.Metodo_Pago) &&
                   PrecioFinal == producto.PrecioFinal &&
                   Apellido_1 == producto.Apellido_1 &&
                   Apellido_2 == producto.Apellido_2 &&
                   EqualityComparer<ApplicationUser>.Default.Equals(ApplicationUser, producto.ApplicationUser) &&
                   EqualityComparer<IList<Producto_Compra>>.Default.Equals(ListaCompra, producto.ListaCompra);
        }
    }
}
