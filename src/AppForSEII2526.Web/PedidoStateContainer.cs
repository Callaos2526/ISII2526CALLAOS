using AppForSEII2526.API.DTOs.PedidoBocaDTOs;

namespace AppForSEII2526.Web
{
    public class PedidoStateContainer
    {
        //Contenedor de estado para el carrito de pedidos (pedido de bocadillos).
        // Mantiene el DTO que se enviará al servidor y expone operaciones comunes (añadir, actualizar, eliminar, vaciar y resetear tras procesar).
        // También expone un evento <see cref="OnChange"/> para que componentes Blazor puedan suscribirse y reaccionar a cambios en el carrito.
        public CreatePedidoDTO Pedido { get; private set; } = new CreatePedidoDTO()
        {
            //DTO que representa el pedido en creación. Se utiliza directamente porque
            // coincide con el contrato que el servidor espera al crear un pedido.
            // Contiene datos del cliente y la lista de "ItemPedidoDTO">.
            BocadilloItem = new List<ItemPedidoDTO>()
        };

        // Precio total del pedido calculado a partir de las líneas (Pvp * Cantidad).
        // Se calcula bajo demanda para reflejar siempre el estado actual de la lista.
        public decimal TotalPrice
        {
            get
            {
                // Convert.ToDecimal se usa para convertir el float Pvp a decimal antes de multiplicar
                return Pedido.BocadilloItem.Sum(i => Convert.ToDecimal(i.Pvp) * i.Cantidad);
            }
        }

        // Evento que notifica a los suscriptores que el estado del pedido ha cambiado.
        // Suscribir componentes Blazor para forzar renderizados o actualizaciones.
        public event Action? OnChange;
        // Método auxiliar para invocar el evento de cambio de estado.
        private void NotifyStateChanged() => OnChange?.Invoke();
        // Añade un bocadillo al pedido a partir de su DTO de selección.
        // - Si el bocadillo ya existe en la lista, incrementa su cantidad en 1.
        // - Si no existe, crea una nueva línea con cantidad inicial 1.
        // Llama a "NotifyStateChanged" al finalizar.

        // Añade un bocadillo a partir del DTO de selección (añade 1 si ya existe)-> DTO con los datos del bocadillo seleccionado
        // <param name="bocadillo">DTO con los datos del bocadillo seleccionado.</param>
        public void AddBocadilloToPedido(SelectBocadilloDTO bocadillo)
        {
            if (bocadillo == null) return;

            var existing = Pedido.BocadilloItem.FirstOrDefault(i => i.ID == bocadillo.BocadilloID);
            if (existing != null)
            {
                //incrementa cantidad si ya estaba en el carrito
                existing.Cantidad += 1;
            }
            else
            {
                // Añade una nueva línea de pedido con cantidad 1
                Pedido.BocadilloItem.Add(new ItemPedidoDTO(
                    bocadillo.BocadilloID,
                    bocadillo.NombreBocadillo,
                    bocadillo.TipoPanNombre,
                    1,
                    bocadillo.Pvp
                ));
            }
            //Añade o actualiza un item en el pedido.
            // - Si ya existe, suma la cantidad proporcionada (útil cuando la UI envía cantidades).
            // - Si no existe, añade una copia del <paramref name="item"/> para evitar aliasing.

            NotifyStateChanged();
        }

        // Añade o incrementa a partir de un ItemPedidoDTO (útil si la UI envía cantidades)
        // <param name="item">Línea de pedido con la cantidad a añadir.</param>
        public void AddOrUpdateItem(ItemPedidoDTO item)
        {
            if (item == null) return;

            var existing = Pedido.BocadilloItem.FirstOrDefault(i => i.ID == item.ID);
            if (existing != null)
            {
                // Suma la cantidad indicada (permite añadir >1 unidades a la vez)
                existing.Cantidad += item.Cantidad;
            }
            else
            {
                // Creamos una nueva instancia para no compartir la referencia del DTO entrante
                Pedido.BocadilloItem.Add(new ItemPedidoDTO(
                    item.ID,
                    item.NombreBocadillo,
                    item.TipoPan,
                    item.Cantidad,
                    item.Pvp
                ));
            }

            NotifyStateChanged();
        }

        // Eliminar un item concreto
        // Se basa en la referencia/igualdad del objeto <see cref="ItemPedidoDTO"/>.
        // <param name="item">Item a eliminar del pedido.</param>
        public void RemoveItem(ItemPedidoDTO item)
        {
            if (item == null) return;

            Pedido.BocadilloItem.Remove(item);
            NotifyStateChanged();
        }

        // Vaciar el carrito de bocadillos 
        public void ClearCart()
        {
            Pedido.BocadilloItem.Clear();
            NotifyStateChanged();
        }

        // Reinicia el estado del pedido una vez que se ha procesado (compra realizada).
        // Crea un nuevo DTO vacío para empezar un nuevo pedido.
        public void PedidoProcessed()
        {
            Pedido = new CreatePedidoDTO()
            {
                BocadilloItem = new List<ItemPedidoDTO>()
            };
            NotifyStateChanged();
        }
    }
}