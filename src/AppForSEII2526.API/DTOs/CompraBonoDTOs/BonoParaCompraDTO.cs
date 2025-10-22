namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class BonoParaCompraDTO
    {
        public BonoParaCompraDTO() { }
        public BonoParaCompraDTO(int bonoID, string nombre, int pvp,  int numeroDeBocadillos, string tipo)
        {
            Nombre = nombre;
            Precio = precio;
            NumeroDeBocadillos = numeroDeBocadillos;
            Tipo = tipo;
        }
        //2.	El Sistema muestra la lista de bonos disponibles, mostrando nombre, precio, número de bocadillos, tipo (veganos, vegetarianos, sin gluten, normal)
        public string Nombre { get; set; }

        public int Precio { get; set; }
        public int NumeroDeBocadillos { get; set; }
        public string Tipo { get; set; }


    }
}
