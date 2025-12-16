using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class ResenyaStateContainer
    {
        public ResenyaForCreateDTO Resenya { get; private set; } = new ResenyaForCreateDTO()
        {
            ResenyaBocadillo = new List<ResenyaItemDTO>()
        };

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        
        public void AddBocadillo(SelectBocadilloDTO bocadillo, int puntuacion)
        {
            if (puntuacion < 1 || puntuacion > 10)
                return;

            var existing = Resenya.ResenyaBocadillo
                .FirstOrDefault(i => i.BocadilloId == bocadillo.BocadilloID);

            if (existing != null)
            {
                existing.Puntuacion = puntuacion;
            }
            else
            {
                Resenya.ResenyaBocadillo.Add(new ResenyaItemDTO
                {
                    BocadilloId = bocadillo.BocadilloID,
                    Nombre = bocadillo.NombreBocadillo,
                    Pvp = bocadillo.Pvp,
                    Tamano = bocadillo.Tamano,
                    Puntuacion = puntuacion
                });
            }

            NotifyStateChanged();
        }

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
