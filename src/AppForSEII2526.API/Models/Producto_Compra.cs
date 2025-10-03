namespace AppForSEII2526.API.Models
{
    public class Producto_Compra
    {
        public Producto_Compra() { }
        public Producto_Compra(int cantidad, int compraid, int productoid, int pvp, int id)
        {
            Cantidad = cantidad;
            Compraid = compraid;
            //Productoid = productoid;
            PVP = pvp;
            Id = id;
        }
        [Key]
        public int Id { get; set; }
        public int Compraid { get; set; }
        //[ForeignKey("Producto")] ??
        public int Productoid { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public int PVP { get; set; }
        public Compra_Producto compra { get; set; }
        public Producto producto { get; set; }
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
