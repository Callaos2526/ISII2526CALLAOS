using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class BonoItemForCreateDTO
    {
        public BonoItemForCreateDTO() { }

        public BonoItemForCreateDTO(int bonoId, int cantidad, string nombre, double precio, int numeroDeBocadillos, string tipo)
        {
            BonoId = bonoId;
            Cantidad = cantidad;
            Nombre = nombre;
            Precio = precio;
            NumeroDeBocadillos = numeroDeBocadillos;
            Tipo = tipo;
        }

        [Required]
        public int BonoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        // Campos informativos que el cliente puede enviar para mostrar en UI.
        // El servidor usará siempre los valores reales de la base de datos para cálculos/validaciones.
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int NumeroDeBocadillos { get; set; }
        public string Tipo { get; set; }
    }
}