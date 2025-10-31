using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.DTOs.PedidoBocaDTOs
{
    
    public class SelectBocadilloDTO
    {
        //Paso 2. El sistema muestra la lista de bocadillos disponibles para pedir en tienda, indicando
        //su nombre, tamaño, tipo de pan y precio.

      
        
        public SelectBocadilloDTO(int bocadilloID,string nombreBocadillo, Tamaño tamano, string tipoPanNombre, float pvp)
        {   
            BocadilloID= bocadilloID;
            NombreBocadillo = nombreBocadillo;
            Tamano = tamano;
            TipoPanNombre = tipoPanNombre; 
            Pvp = pvp;
        }
        
        [Key]
        public int BocadilloID { get; set; }
        public string NombreBocadillo { get; set; }
        public Tamaño Tamano { get; set; }
        public string TipoPanNombre { get; set; } //dice noelia que bien que sea string y no objeto tipopan
        public float Pvp { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SelectBocadilloDTO dTO &&
                   BocadilloID == dTO.BocadilloID &&
                   NombreBocadillo == dTO.NombreBocadillo &&
                   Tamano == dTO.Tamano &&
                   TipoPanNombre == dTO.TipoPanNombre &&
                   Pvp == dTO.Pvp;
        }
    }
}
