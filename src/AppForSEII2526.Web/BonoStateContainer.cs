
using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class BonoStateContainer
    {
        public CrearCompraDTO Compra { get; private set; } = new CrearCompraDTO()
        {
            BonoItem = new List<BonoItemForCreateDTO>()
        };

        public decimal TotalPrice
        {
            get
            {
                return Compra.BonoItem.Sum(i => Convert.ToDecimal(i.Precio) * i.Cantidad);
            }
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddProduct(SelectBonoDTO item)
        {
            if (item == null) return;

            // Si ya existe, incrementa cantidad; si no, lo añade
            var existing = Compra.BonoItem.FirstOrDefault(i => i.BonoId == item.BonoId);
            if (existing != null)
            {
                existing.Cantidad += 1;
            }
            else
            {
                Compra.BonoItem.Add(new BonoItemForCreateDTO
                {
                    BonoId = item.BonoId,
                    Cantidad = 1,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    NumeroDeBocadillos = item.NumeroDeBocadillos,
                    Tipo = item.Tipo
                });
            }

            NotifyStateChanged();
        }
        

        // Eliminar un producto concreto del carrito
        public void RemoveItem(BonoItemForCreateDTO item)
        {
            Compra.BonoItem.Remove(item);
        }

        // Vaciar el carrito
        public void ClearCart()
        {
            Compra.BonoItem.Clear();
        }

        // Reset del estado cuando la compra se ha procesado
        public void CompraProcessed()
        {
            Compra = new CrearCompraDTO()
            {
                BonoItem = new List<BonoItemForCreateDTO>()
            };
        }
    }
}
