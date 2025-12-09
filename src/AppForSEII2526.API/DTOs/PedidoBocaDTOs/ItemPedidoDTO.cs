using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.PedidoBocaDTOs
{
    public class ItemPedidoDTO
    {
        public ItemPedidoDTO(int id, string nombreBocadillo, string tipoPan, int cantidad, float pvp)
        {
            Id = id;
            NombreBocadillo = nombreBocadillo;
            TipoPan = tipoPan;
            Cantidad = cantidad;
            Pvp = pvp;
        }

        // Cambiado a 'Id' (coincidir con el resto del código)
        public int Id { get; set; }

        public string NombreBocadillo { get; set; }
        public string TipoPan { get; set; }

        [Required]
        public int Cantidad { get; set; }

        public float Pvp { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ItemPedidoDTO dTO &&
                   Id == dTO.Id &&
                   NombreBocadillo == dTO.NombreBocadillo &&
                   TipoPan == dTO.TipoPan &&
                   Cantidad == dTO.Cantidad &&
                   Pvp == dTO.Pvp;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, NombreBocadillo, TipoPan, Cantidad, Pvp);
        }
    }
}
