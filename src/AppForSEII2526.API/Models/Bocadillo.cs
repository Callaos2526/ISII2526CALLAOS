using NuGet.Versioning;


namespace AppForSEII2526.API.Models
{
   
    public class Bocadillo
    {
        
        public Bocadillo() { }
        
        public Bocadillo(int id, string nombre, float pvp, string resenyabocadillo, int stock, string tamano, int comprasDelBocadillo)
        {
            ComprasDelBocadillo = comprasDelBocadillo;
            

            Id = id;
            Nombre = nombre;
            Pvp = pvp;
            Resenyabocadillo = resenyabocadillo;
            Stock = stock;
            Tamano = tamano;

            
        }
        [Required]
        public int ComprasDelBocadillo { get; set; }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public float Pvp { get; set; }
        [Required]
        public string Resenyabocadillo { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public string Tamano { get; set; }


        public Tamaño tamaño { get; set; }

   
        public TipoPan tipopan { get; set; }
        public IList<ResenyaBocadillo> ResenyaBocadillo { get; set; } = new List<ResenyaBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   ComprasDelBocadillo == bocadillo.ComprasDelBocadillo &&
                   Id == bocadillo.Id &&
                   Nombre == bocadillo.Nombre &&
                   Pvp == bocadillo.Pvp &&
                   ResenyaBocadillo == bocadillo.ResenyaBocadillo &&
                   Stock == bocadillo.Stock;
                   
                   
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, Pvp, ResenyaBocadillo, Stock, Tamano, ComprasDelBocadillo);
        }
    }
}
