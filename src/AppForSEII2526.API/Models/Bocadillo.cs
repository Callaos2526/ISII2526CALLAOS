
namespace AppForSEII2526.API.Models
{
    public class Bocadillo
    {
        public Bocadillo(int id, string Nombre ,int pvp, string resenyaBocadillo,int stock,int tamaño)
        {
            Id = id;
            nombre = Nombre;
            PVP= pvp;
            ResenyaBocadillo = resenyaBocadillo;
            Stock = stock;
            Tamaño = tamaño;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string nombre { get; set; }

        [Required]
        public int PVP { get; set; }

        [Required]
        public string ResenyaBocadillo { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int Tamaño { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   Id == bocadillo.Id &&
                   nombre == bocadillo.nombre &&
                   PVP == bocadillo.PVP &&
                   ResenyaBocadillo == bocadillo.ResenyaBocadillo &&
                   Stock == bocadillo.Stock &&
                   Tamaño == bocadillo.Tamaño;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, nombre, PVP, ResenyaBocadillo, Stock, Tamaño);
        }
    }
}
