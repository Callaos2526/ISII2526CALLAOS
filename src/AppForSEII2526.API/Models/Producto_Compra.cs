namespace AppForSEII2526.API.Models
{
    public class Producto_Compra
    {
        public Producto_Compra(int cantidad, int compraid, int productoid, int pvp)
        {
            Cantidad = cantidad;
            Compraid = compraid;
            //Productoid = productoid;
            PVP = pvp;
        }
        [Key]
        public int Compraid { get; set; }
        //[ForeignKey("Producto")] ??
        //public Productoid { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public int PVP { get; set; }
        IList<Producto_Compra> productos_compras = new List<Producto_Compra>();
        public override bool Equals(object? obj)
        {
            return obj is Producto_Compra producto_compra &&
                   Compraid == producto_compra.Compraid &&
                   //Productoid == producto_compra.Productoid &&
                   Cantidad == producto_compra.Cantidad &&
                   PVP == producto_compra.PVP;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Compraid, /*Productoid,*/ Cantidad, PVP);
        }
    }
}
