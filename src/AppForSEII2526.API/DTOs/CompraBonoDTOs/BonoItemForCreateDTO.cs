using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CompraBonoDTOs
{
    public class BonoItemForCreateDTO
    {
        public BonoItemForCreateDTO() 
        { 
            Nombre = string.Empty;
            Tipo = string.Empty;
        }

        public BonoItemForCreateDTO(int bonoId, int cantidad, string nombre, double precio, int numeroDeBocadillos, string tipo)
        {
            BonoId = bonoId;
            Cantidad = cantidad;
            Nombre = nombre ?? string.Empty;
            Precio = precio;
            NumeroDeBocadillos = numeroDeBocadillos;
            Tipo = tipo ?? string.Empty;
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

        public override bool Equals(object? obj)
        {
            return obj is BonoItemForCreateDTO dTO &&
                   BonoId == dTO.BonoId &&
                   Cantidad == dTO.Cantidad &&
                   string.Equals(Nombre, dTO.Nombre, StringComparison.Ordinal) &&
                   Math.Abs(Precio - dTO.Precio) < 0.01 &&
                   NumeroDeBocadillos == dTO.NumeroDeBocadillos &&
                   string.Equals(Tipo, dTO.Tipo, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BonoId, Cantidad, Nombre, Precio, NumeroDeBocadillos, Tipo);
        }
    }
}