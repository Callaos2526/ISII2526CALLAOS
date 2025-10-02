namespace AppForSEII2526.API.Models
{
    public class BonoBocadillo
    {
        //Constructor vacío
        public BonoBocadillo()
        {
        }
        public BonoBocadillo(int bonoId, int cantidadDisponible, int nBocadillos, string nombre, int pvp, TipoBocadillo tipoBocadillo)
        {
            BonoId = bonoId;
            CantidadDisponible = cantidadDisponible;
            NBocadillos = nBocadillos;
            Nombre = nombre;
            PVP = pvp;
            TipoBocadillos = tipoBocadillo;
        }
        
        [Key]
        public int BonoId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CantidadDisponible { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int NBocadillos { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public double PVP { get; set; }
        [Required]
        public TipoBocadillo TipoBocadillos { get; set; }
        [Required]
        public IList<BonosComprados> bonosComprados { get; set; } = new List<BonosComprados>();

        public override bool Equals(object? obj)
        {
            return obj is BonoBocadillo bocadillo &&
                   BonoId == bocadillo.BonoId &&
                   CantidadDisponible == bocadillo.CantidadDisponible &&
                   NBocadillos == bocadillo.NBocadillos &&
                   Nombre == bocadillo.Nombre &&
                   PVP == bocadillo.PVP &&
                   EqualityComparer<TipoBocadillo>.Default.Equals(TipoBocadillos, bocadillo.TipoBocadillos) &&
                   EqualityComparer<IList<BonosComprados>>.Default.Equals(bonosComprados, bocadillo.bonosComprados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BonoId, CantidadDisponible, NBocadillos, Nombre, PVP, TipoBocadillos, bonosComprados);
        }
    }
}
