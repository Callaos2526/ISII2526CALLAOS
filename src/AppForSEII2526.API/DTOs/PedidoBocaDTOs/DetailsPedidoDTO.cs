using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.PedidoBocaDTOs
{
    public class DetailsPedidoDTO : CreatePedidoDTO
    {  //details es solo lectura no hay required

        //Paso 7.El sistema muestra el pedido realizado, indicando los datos del cliente (Nombre y 
        //Apellidos), cuándo se realizó el pedido, su precio total y los bocadillos pedidos
        //(nombre, precio y tipo de pan y cantidad). 


        public DetailsPedidoDTO(int id, DateTime fechaPedido, string nombreCliente, string apellidoCliente1, string? apellidoCliente2,
            string metodo, IList<ItemPedidoDTO> bocadilloItem)
               : base(nombreCliente, apellidoCliente1, apellidoCliente2, metodo, bocadilloItem) 
        {   //inicializo los atributos de la clase hija que no estan en la clase padre
            Id = id;
            FechaPedido = fechaPedido;
            
        }
        
        public int Id { get; set; }
        //Darle formato para que solo entre dia mes y año
         [Display(Name = "Fecha de pedido")]
        public DateTime FechaPedido { get; set; }

       
        public override bool Equals(object? obj)
        {
            return obj is DetailsPedidoDTO dTO &&
                   Id == dTO.Id &&
                   FechaPedido == dTO.FechaPedido &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente1 == dTO.ApellidoCliente1 &&
                   ApellidoCliente2 == dTO.ApellidoCliente2 &&
                   Metodo == dTO.Metodo &&
                   // comprueba que ambas colecciones son iguales (SequenceEqual requiere ItemPedidoDTO.Equals)
                   BocadilloItem.SequenceEqual(dTO.BocadilloItem);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCliente, ApellidoCliente1, ApellidoCliente2, Metodo, BocadilloItem, Id, FechaPedido);
        }
    }
        
        






    
}
