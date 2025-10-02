

namespace AppForSEII2526.API.Models
{
    public class BonosComprados
    {
        //noelia.gonzales@uclm.es
        //Constructor vacío
        public BonosComprados()
        {
        }
        public BonosComprados(int id, int bonoId, int cantidad, int compraId, double precioBono)
        {
            Id = id;
            BonoId = bonoId;
            Cantidad = cantidad;
            CompraId = compraId;
            PrecioBono = precioBono;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public int BonoId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
        [Required]
        public int CompraId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public double PrecioBono { get; set; }
       
        
        public BonosComprados Bono { get; set; }
        
        public CompraBono Compra { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BonosComprados comprados &&
                   Id == comprados.Id &&
                   BonoId == comprados.BonoId &&
                   Cantidad == comprados.Cantidad &&
                   CompraId == comprados.CompraId &&
                   PrecioBono == comprados.PrecioBono &&
                   EqualityComparer<BonosComprados>.Default.Equals(Bono, comprados.Bono) &&
                   EqualityComparer<CompraBono>.Default.Equals(Compra, comprados.Compra);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BonoId, Cantidad, CompraId, PrecioBono, Bono, Compra);
        }
    }
}
