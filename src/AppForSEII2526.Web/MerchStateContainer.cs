
using AppForSEII2526.API.DTOs.ComprarMerch;

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
                return Compra.MerchItems.Sum(i => Convert.ToDecimal(i.PVP) * i.Cantidad);
            }
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddProduct(ComprarMerchItemDTO item)
        {
            if (item == null) return;

            // Si ya existe, incrementa cantidad; si no, lo añade
            var existing = Compra.MerchItems.FirstOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                existing.Cantidad += item.Cantidad;
            }
            else
            {
                Compra.MerchItems.Add(new ComprarMerchItemDTO(
                    item.Id,
                    item.NombreProducto,
                    item.PVP,
                    item.TipoProducto,
                    item.Cantidad
                ));
            }

            NotifyStateChanged();
        }

        // Eliminar un producto concreto del carrito
        public void RemoveItem(ComprarMerchItemDTO item)
        {
            Compra.MerchItems.Remove(item);
        }

        // Vaciar el carrito
        public void ClearCart()
        {
            Compra.MerchItems.Clear();
        }

        // Reset del estado cuando la compra se ha procesado
        public void CompraProcessed()
        {
            Compra = new ComprarMerchCreateDTO()
            {
                MerchItems = new List<ComprarMerchItemDTO>()
            };
        }
    }
}
