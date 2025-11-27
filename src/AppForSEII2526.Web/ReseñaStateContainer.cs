using AppForSEII2526.API.DTOs.ResenyaDTOs;

namespace AppForSEII2526.Web
{
    public class ResenyaStateContainer
    {
        // La reseña que se está creando
        public ResenyaForCreateDTO Resenya { get; private set; } = new ResenyaForCreateDTO()
        {
            ResenyaBocadillo = new List<ResenyaItemDTO>()
        };

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        //   AÑADIR BOCADILLOS A LA RESEÑA
        public void AddBocadillo(int bocadilloId, int puntuacion)
        {
            if (puntuacion < 1 || puntuacion > 10)
                return;

            var existing = Resenya.ResenyaBocadillo
                .FirstOrDefault(i => i.BocadilloId == bocadilloId);

            if (existing != null)
            {
                // Si ya existe ese bocadillo, solo se actualiza la puntuación
                existing.Puntuacion = puntuacion;
            }
            else
            {
                // Si no existe, se añade uno nuevo
                Resenya.ResenyaBocadillo.Add(new ResenyaItemDTO(
                    bocadilloId,
                    puntuacion
                ));
            }

            NotifyStateChanged();
        }
        //   ELIMINAR BOCADILLO
        public void RemoveBocadillo(ResenyaItemDTO item)
        {
            Resenya.ResenyaBocadillo.Remove(item);
            NotifyStateChanged();
        }

        public void ClearResenya()
        {
            Resenya.ResenyaBocadillo.Clear();
            NotifyStateChanged();
        }

        public void ResenyaProcessed()
        {
            Resenya = new ResenyaForCreateDTO()
            {
                ResenyaBocadillo = new List<ResenyaItemDTO>()
            };

            NotifyStateChanged();
        }
    }
}

