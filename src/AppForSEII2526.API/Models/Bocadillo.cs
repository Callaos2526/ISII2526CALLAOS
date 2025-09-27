

namespace AppForSEII2526.API.Models
{
    public class Bocadillo
    {
        public Bocadillo(int comprasDelBocadillo, int id, string Nombre ,int pvp,int Stock)
        {
            ComprasDelBocadillo = comprasDelBocadillo;
            Id = id;
            nombre = Nombre;
            PVP= pvp;
            stock = Stock;
        }
        [Required]
        public int ComprasDelBocadillo { get; set; }
        [Key]
        public int Id { get; set; }
        [Required]
        public string nombre { get; set; }

        [Required]
        public int PVP { get; set; }

        [Required]
        public int stock { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   ComprasDelBocadillo == bocadillo.ComprasDelBocadillo &&
                   Id == bocadillo.Id &&
                   nombre == bocadillo.nombre &&
                   PVP == bocadillo.PVP &&
                   stock == bocadillo.stock;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ComprasDelBocadillo, Id, nombre, PVP, stock);
        }
    }
}
