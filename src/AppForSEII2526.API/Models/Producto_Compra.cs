namespace AppForSEII2526.API.Models
{
    public class Producto_Compra
    {
        public Producto_Compra() { }
        public Producto_Compra(int id, int compraid, int cantidad, int productoid, double pvp)
        {
            Id = id;
            Compraid = compraid;
            Cantidad = cantidad;
            Productoid = productoid;
            PVP = pvp;
        }
        [Key]
        public int Id { get; set; }
        public int Compraid { get; set; }
        public int Productoid { get; set; }
        public double PVP { get; set; }
        public int Cantidad { get; set; }
        public Compra_Producto Compra { get; set; }
        public Producto Producto { get; set; }
        public override bool Equals(object? obj)    
        {
            return obj is Producto_Compra producto_compra &&
                   Compraid == producto_compra.Compraid &&
                   Productoid == producto_compra.Productoid &&
                   Cantidad == producto_compra.Cantidad &&
                   PVP == producto_compra.PVP;
        }
        
    }
}
