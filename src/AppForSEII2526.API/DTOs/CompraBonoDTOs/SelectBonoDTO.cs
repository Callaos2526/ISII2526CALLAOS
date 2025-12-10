namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class SelectBonoDTO
    {
        public SelectBonoDTO() { }
        public SelectBonoDTO(int bonoID, string nombre, double pvp,  int numeroDeBocadillos, string tipo)
        {
            BonoID = bonoID;
            Nombre = nombre;
            Precio = pvp;
            NumeroDeBocadillos = numeroDeBocadillos;
            Tipo = tipo;
        }
        //2.	El Sistema muestra la lista de bonos disponibles, mostrando nombre, precio, número de bocadillos, tipo (veganos, vegetarianos, sin gluten, normal)
        public int BonoID { get; set; }
        public string Nombre { get; set; }

        public double Precio { get; set; }
        public int NumeroDeBocadillos { get; set; }
        public string Tipo { get; set; }


    }
}
