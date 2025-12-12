using OpenQA.Selenium;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AppForSEII2526.UIT.Resenya
{
    public class DetailResenya_PO : PageObject
    {
        // Elementos de la página de detalle de la reseña
        private By _nombreUsuario = By.Id("NombreUsuario");
        private By _fechaPublicacion = By.Id("FechaPublicacion");
        private By _titulo = By.Id("Titulo");
        private By _descripcion = By.Id("Descripcion");
        private By _valoracion = By.Id("Valoracion");

        private By _tablaResenyaBocadillos = By.Id("ResenyaBocadillos");
        private By _mensajeError = By.Id("ErrorMessage");

        public DetailResenya_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        // Verifica si la información de la reseña es correcta
        public bool VerificarDetalleResenya(string nombreUsuario, string fechaPublicacion, string titulo, string descripcion, string valoracion)
        {
            try
            {
                // Esperar a que la página esté renderizada
                WaitForBeingVisible(_nombreUsuario);

                var actualNombreUsuario = _driver.FindElement(_nombreUsuario).Text.Trim();
                var actualFechaPublicacion = _driver.FindElement(_fechaPublicacion).Text.Trim();
                var actualTitulo = _driver.FindElement(_titulo).Text.Trim();
                var actualDescripcion = _driver.FindElement(_descripcion).Text.Trim();
                var actualValoracion = _driver.FindElement(_valoracion).Text.Trim();

                _output.WriteLine($"Detalle obtenido: Nombre='{actualNombreUsuario}', Fecha='{actualFechaPublicacion}', Titulo='{actualTitulo}', Valoracion='{actualValoracion}'");

                // Comparaciones tolerantes
                bool okNombre = string.IsNullOrEmpty(nombreUsuario) || actualNombreUsuario.Contains(nombreUsuario, StringComparison.InvariantCultureIgnoreCase);

                bool okTitulo = string.IsNullOrEmpty(titulo) || actualTitulo.Contains(titulo, StringComparison.InvariantCultureIgnoreCase);

                bool okDescripcion = string.IsNullOrEmpty(descripcion) || actualDescripcion.Contains(descripcion, StringComparison.InvariantCultureIgnoreCase);

                bool okValoracion = string.IsNullOrEmpty(valoracion) || actualValoracion.Contains(valoracion, StringComparison.InvariantCultureIgnoreCase);

                bool okFecha = true;
                if (!string.IsNullOrEmpty(fechaPublicacion))
                {
                    // Intentar comparar contenido o parsear fechas si el formato coincide
                    if (DateTime.TryParseExact(fechaPublicacion, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expectedDt) &&
                        DateTime.TryParseExact(actualFechaPublicacion, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var actualDt))
                    {
                        okFecha = expectedDt == actualDt;
                    }
                    else
                    {
                        // Fallback: contains (más tolerante)
                        okFecha = actualFechaPublicacion.Contains(fechaPublicacion, StringComparison.InvariantCultureIgnoreCase);
                    }
                }

                return okNombre && okFecha && okTitulo && okDescripcion && okValoracion;
            }
            catch (NoSuchElementException ex)
            {
                _output.WriteLine($"Elemento no encontrado al verificar detalle reseña: {ex.Message}");
                return false;
            }
            catch (WebDriverException ex)
            {
                _output.WriteLine($"Error WebDriver al verificar detalle reseña: {ex.Message}");
                return false;
            }
        }

        // Verifica la tabla de bocadillos puntuados
        public bool VerificarBocadillosPuntuados(List<string[]> expectedRows)
        {
            try
            {
                WaitForBeingVisible(_tablaResenyaBocadillos);
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Timeout esperando la tabla de bocadillos en detalle de reseña.");
                return false;
            }

            // Reutiliza la comprobación genérica (devuelve logs más detallados si falla)
            return CheckBodyTable(expectedRows, _tablaResenyaBocadillos);
        }

        // Verifica si el mensaje de error es visible
        public bool VerificarMensajeError(string expectedError)
        {
            try
            {
                WaitForBeingVisible(_mensajeError);
                var actualError = _driver.FindElement(_mensajeError).Text;
                _output.WriteLine($"Mensaje de error mostrado: {actualError}");
                return string.IsNullOrEmpty(expectedError) || actualError.Contains(expectedError, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (NoSuchElementException)
            {
                _output.WriteLine("No se encontró el mensaje de error.");
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Timeout esperando mensaje de error.");
                return false;
            }
        }
    }
}
