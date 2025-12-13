using AppForSEII2526.Web.API;
namespace AppForSEII2526.Web
{
    public class CompraMerchStateContainer
    {
        public ComprarMerchCreateDTO Compra { get; private set; } = new ComprarMerchCreateDTO()
        {
            MerchItems = new List<ComprarMerchItemDTO>()
        };

        public decimal TotalPrice
        {
            get           
            {
                return Compra.MerchItems.Sum(i => Convert.ToDecimal(i.Pvp) * i.Cantidad);
            }
        }
        public bool HasItems => Compra.MerchItems.Any();

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddProduct(ComprarMerchandaisingDTO item)
        {
            if (item == null) return;

            var existing = Compra.MerchItems.FirstOrDefault(i => i.Id == item.ProductoId);
            if (existing != null)
            {
                // sumar de uno en uno
                existing.Cantidad += 1;
            }
            else
            {
                Compra.MerchItems.Add(new ComprarMerchItemDTO
                {
                    Id = item.ProductoId,
                    NombreProducto = item.NombreProducto,
                    Pvp = item.Pvp,
                    TipoProducto = item.TipoProducto,
                    Cantidad = 1
                });
            }

            NotifyStateChanged();
        }

        // Actualiza la cantidad de un producto en el carrito
        public void UpdateQuantity(int productId, int newQuantity)
        {
            var existing = Compra.MerchItems.FirstOrDefault(i => i.Id == productId);
            if (existing != null)
            {
                if (newQuantity <= 0)
                {
                    Compra.MerchItems.Remove(existing); // Elimina si la cantidad es 0 o negativa
                }
                else
                {
                    existing.Cantidad = newQuantity;
                }
                NotifyStateChanged();
            }
        }

        // Eliminar un producto concreto del carrito
        public void RemoveItem(ComprarMerchItemDTO item)
        {
            if (item != null && Compra.MerchItems.Contains(item))
            {
                Compra.MerchItems.Remove(item);
                NotifyStateChanged();
            }
        }
        public void RemoveItemById(int productId)
        {
            var item = Compra.MerchItems.FirstOrDefault(i => i.Id == productId);
            if (item != null)
            {
                Compra.MerchItems.Remove(item);
                NotifyStateChanged();
            }
        }

        // Vaciar el carrito
        public void ClearCart()
        {
            Compra.MerchItems.Clear();
            NotifyStateChanged();
        }
        public void SetClientData(string nombre, string apellido1, string apellido2,
                                   string direccion, string metodoPago)
        {
            Compra.Nombre = nombre;
            Compra.Apellido_1 = apellido1;
            Compra.Apellido_2 = apellido2; // Opcional
            Compra.Direccion_Envio = direccion;
            Compra.Metodo_Pago = metodoPago;

            NotifyStateChanged();
        }

        public void compraProcesada()
        {
            Compra = new ComprarMerchCreateDTO()
            {
                MerchItems = new List<ComprarMerchItemDTO>()
            };
            NotifyStateChanged();
        }

    }
}
