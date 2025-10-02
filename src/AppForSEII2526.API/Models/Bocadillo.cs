using NuGet.Versioning;

namespace AppForSEII2526.API.Models
{
   
    public class Bocadillo
    {
        
        public Bocadillo() { }
        public Bocadillo(int id, string nombre, float pvp, string resenyabocadillo, int stock, string tamano)
        {        
            

            Id = id;
            Nombre = nombre;
            Pvp = pvp;
            ResenyaBocadillo = resenyabocadillo;
            Stock = stock;
            Tamano = tamano;

            
        }
        [Key] 
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public float Pvp { get; set; }
        [Required]
        public string ResenyaBocadillo { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public string Tamano { get; set; }

            //relacion con TipoPan
        [Required]
        public TipoPan TipoPan { get; set; }

        //Vector de relacion 1:N con CompraBocadillo
        [Required]
        public IList<CompraBocadillo> ComprasDelBocadillo { get; set; } =new List<CompraBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   Id == bocadillo.Id &&
                   Nombre == bocadillo.Nombre &&
                   Pvp == bocadillo.Pvp &&
                   ResenyaBocadillo == bocadillo.ResenyaBocadillo &&
                   Stock == bocadillo.Stock &&
                   Tamano == bocadillo.Tamano &&
                   EqualityComparer<TipoPan>.Default.Equals(TipoPan, bocadillo.TipoPan) &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(ComprasDelBocadillo, bocadillo.ComprasDelBocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, Pvp, ResenyaBocadillo, Stock, Tamano, TipoPan, ComprasDelBocadillo);
        }
    }
}
