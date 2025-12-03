using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    // Versión con la misma estructura/estilo que usa tu profesora:
    // - usa los DTO generados por el cliente (AppForSEII2526.Web.API)
    // - expone la propiedad CreatePedidoDTO `Pedido`
    // - expone `OnChange` pero NO lo invoca internamente (igual que tu profesora)
    public class PedidoStateContainer
    {
        public CreatePedidoDTO Pedido { get; private set; } = new CreatePedidoDTO()
        {
            BocadilloItem = new List<ItemPedidoDTO>()
        };

        public decimal TotalPrice
        {
            get
            {
                // Protección contra null y conversión explícita a decimal
                return Pedido?.BocadilloItem?.Sum(i => Convert.ToDecimal(i.Pvp) * i.Cantidad) ?? 0m;
            }
        }

        // Evento público, igual que en el ejemplo de la profesora (no se dispara automáticamente aquí)
        public event Action? OnChange;
        // Método helper para disparar el evento cuando la UI/servicio lo desee
        public void NotifyStateChanged() => OnChange?.Invoke();

        // Añade un bocadillo (si ya existe incrementa cantidad en 1)
        public void AddBocadilloToPedido(SelectBocadilloDTO bocadillo)
        {
            if (bocadillo == null) return;
        
            var existing = Pedido.BocadilloItem.FirstOrDefault(i => i.Id == bocadillo.BocadilloID);
            if (existing != null)
            {
                existing.Cantidad += 1;
            }
            else
            {
                Pedido.BocadilloItem ??= new List<ItemPedidoDTO>();
                // Uso inicializador de objeto para evitar dependencias de constructores concretos del DTO cliente
                Pedido.BocadilloItem.Add(new ItemPedidoDTO
                {
                    Id = bocadillo.BocadilloID,
                    NombreBocadillo = bocadillo.NombreBocadillo,
                    TipoPan = bocadillo.TipoPanNombre,
                    Cantidad = 1,
                    Pvp = bocadillo.Pvp
                });
            }

            // Igual que la profesora: no invocar OnChange/NotifyStateChanged aquí.
        }

        // Añade o actualiza una línea (la UI puede usar esto para añadir cantidades específicas)
        public void AddOrUpdateItem(ItemPedidoDTO item)
        {
            if (item == null) return;

            Pedido.BocadilloItem ??= new List<ItemPedidoDTO>();
            var existing = Pedido.BocadilloItem.FirstOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                existing.Cantidad += item.Cantidad;
            }
            else
            {
                Pedido.BocadilloItem.Add(new ItemPedidoDTO
                {
                    Id = item.Id,
                    NombreBocadillo = item.NombreBocadillo,
                    TipoPan = item.TipoPan,
                    Cantidad = item.Cantidad,
                    Pvp = item.Pvp
                });
            }            
        }

        // Eliminar un item concreto (método público usado por la UI)
        public void RemoveItem(ItemPedidoDTO item)
        {
            if (item == null || Pedido?.BocadilloItem == null) return;

            // Eliminar por Id (más robusto que por referencia)
            var existing = Pedido.BocadilloItem.FirstOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                Pedido.BocadilloItem.Remove(existing);
            }
        }

        // Vaciar el carrito
        public void ClearCart()
        {
            Pedido.BocadilloItem.Clear();
  
        }

        // Reset cuando el pedido se ha procesado
        public void PedidoProcessed()
        {
            Pedido = new CreatePedidoDTO()
            {
                BocadilloItem = new List<ItemPedidoDTO>()
            };
            
        }
    }
}