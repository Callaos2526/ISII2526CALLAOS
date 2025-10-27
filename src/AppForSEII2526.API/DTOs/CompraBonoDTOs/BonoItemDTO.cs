namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class BonoItemDTO
    {
        public BonoItemDTO() { }
        public BonoItemDTO(int id, string nombre, double pvp, int numeroDeBocadillos, TipoBocadillo tipo)
        {
            ID = id;
            Nombre = nombre;
            Precio = pvp;
            NumeroDeBocadillos = numeroDeBocadillos;
            Tipo = tipo;
        }
        [Key]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int NumeroDeBocadillos { get; set; }
        public TipoBocadillo Tipo { get; set; }
    }
}
