
namespace AppForSEII2526.API.DTOs.PedidoBocaDTOs
{
    public class ItemPedidoDTO

    //PASO 5. El sistema muestra la lista de bocadillos seleccionados incluyendo su nombre, precio 
    //y tipo de pan y solicita la cantidad a comprar de cada bocadillo, siendo este campo
    //obligatorio.

    {
        public ItemPedidoDTO(int id, string nombreBocadillo, string tipoPan, int cantidad, float pvp) //preguntar si la cantidad va aqui
        {
            ID= id;
            NombreBocadillo = nombreBocadillo;
            TipoPan = tipoPan;
            Cantidad = cantidad;
            Pvp = pvp;
        }
       
        public int ID { get; set; }
        public string NombreBocadillo { get; set; }
        public string TipoPan { get; set; }
        [Required]
        public int Cantidad { get; set; }
        public float Pvp { get; set; }

        public bool Equals(ItemPedidoDTO? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            const float EPS = 0.001f;
            return ID == other.ID
                && string.Equals(NombreBocadillo, other.NombreBocadillo, StringComparison.Ordinal)
                && string.Equals(TipoPan, other.TipoPan, StringComparison.Ordinal)
                && Cantidad == other.Cantidad
                && MathF.Abs(Pvp - other.Pvp) < EPS;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, NombreBocadillo, TipoPan, Cantidad, Pvp);
        }
        //**
    }
}
