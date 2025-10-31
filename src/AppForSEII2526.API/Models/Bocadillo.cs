using NuGet.Versioning;

namespace AppForSEII2526.API.Models
{
   
    public class Bocadillo
    {
        
        public Bocadillo() { }
        
        public Bocadillo(int id, string nombre, float pvp, string resenyabocadillo, int stock, Tamaño tamano, int comprasDelBocadillo)
        {
            ComprasDelBocadillo = comprasDelBocadillo;
            

            Id = id;
            Nombre = nombre;
            Pvp = pvp;
            Resenyabocadillo = resenyabocadillo;
            Stock = stock;
            Tamano = tamano;

            
        }
        public int ComprasDelBocadillo { get; set; }
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public float Pvp { get; set; }
        public string Resenyabocadillo { get; set; }
        public int Stock { get; set; }
       // public string Tamano { get; set; }

        public Tamaño Tamano { get; set; }

   
        public TipoPan tipopan { get; set; }
        public IList<ResenyaBocadillo> ResenyaBocadillo { get; set; } = new List<ResenyaBocadillo>();
        public IList<CompraBocadillo> ComprasBocadillo { get; set; } = new List<CompraBocadillo>();

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   ComprasDelBocadillo == bocadillo.ComprasDelBocadillo &&
                   Id == bocadillo.Id &&
                   Nombre == bocadillo.Nombre &&
                   Pvp == bocadillo.Pvp &&
                   Resenyabocadillo == bocadillo.Resenyabocadillo &&
                   Stock == bocadillo.Stock &&
                   Tamano == bocadillo.Tamano &&
                   EqualityComparer<TipoPan>.Default.Equals(tipopan, bocadillo.tipopan) &&
                   EqualityComparer<IList<ResenyaBocadillo>>.Default.Equals(ResenyaBocadillo, bocadillo.ResenyaBocadillo) &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(ComprasBocadillo, bocadillo.ComprasBocadillo);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ComprasDelBocadillo);
            hash.Add(Id);
            hash.Add(Nombre);
            hash.Add(Pvp);
            hash.Add(Resenyabocadillo);
            hash.Add(Stock);
            hash.Add(Tamano);
            hash.Add(tipopan);
            hash.Add(ResenyaBocadillo);
            hash.Add(ComprasBocadillo);
            return hash.ToHashCode();
        }
    }
}
   
