

namespace AppForSEII2526.API.Models
{
    public class BonosComprados
    {
        //noelia.gonzales@uclm.es
        //Constructor vacío
        public BonosComprados()
        {
        }
        public BonosComprados(int id, int bonoId, int cantidad, int compraId, double precioBono, BonoBocadillo bonos, CompraBono compra)
        {
            Id = id;
            BonoId = bonoId;
            Cantidad = cantidad;
            CompraId = compraId;
            Cantidad = cantidad;
            PrecioBono = precioBono;
            Bono = bonos;
            Compra = compra;
        }
        [Key]
        public int Id { get; set; }
        [ForeignKey("CompraId")]
        public int CompraId { get; set; }

        [ForeignKey("BonoId")]
        public int BonoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
       
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public double PrecioBono { get; set; }

        public BonoBocadillo Bono { get; set; }

        public CompraBono Compra { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BonosComprados comprados &&
                   Id == comprados.Id &&
                   BonoId == comprados.BonoId &&
                   Cantidad == comprados.Cantidad &&
                   CompraId == comprados.CompraId &&
                   PrecioBono == comprados.PrecioBono &&
                   EqualityComparer<BonoBocadillo>.Default.Equals(Bono, comprados.Bono) &&
                   EqualityComparer<CompraBono>.Default.Equals(Compra, comprados.Compra);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BonoId, Cantidad, CompraId, PrecioBono, Bono, Compra);
        }
    }
}
