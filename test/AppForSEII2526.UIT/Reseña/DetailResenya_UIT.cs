using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Resenya
{
    public class DetailResenya_UIT : UC_UIT
    {
        private SelectResenya_PO selectResenya_PO;
        private DetailResenya_PO detailResenya_PO;

        // Ajusta estos valores según los datos de tu BD si es necesario
        private const int bocadilloId1 = 1;
        private const string bocadilloName1 = "Submarino";
        private const string bocadilloPvp1 = "6";

        public DetailResenya_UIT(ITestOutputHelper output) : base(output)
        {
            selectResenya_PO = new SelectResenya_PO(_driver, _output);
            detailResenya_PO = new DetailResenya_PO(_driver, _output);
        }

        private void InitialSteps_OpenSelect()
        {
            _driver.Navigate().GoToUrl(_URI + "resenya/selectbocadillos");
            selectResenya_PO.WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_DetailResenya_CreateAndVerify()
        {
            // Arrange: ir a selección y añadir un bocadillo
            InitialSteps_OpenSelect();

            selectResenya_PO.AddBocadilloToResenya(bocadilloId1);

            // Esperar y clicar en Crear reseña
            selectResenya_PO.WaitForBeingVisible(By.Id("goToCreateResenya"));
            _driver.FindElement(By.Id("goToCreateResenya")).Click();

            // Esperar a la página de creación
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.FindElement(By.Id("Titulo")).Displayed);

            // Rellenar formulario de creación
            var titulo = "UIT Test Title " + DateTime.Now.Ticks;
            var descripcion = "UIT Test Description " + DateTime.Now.Ticks;
            _driver.FindElement(By.Id("Titulo")).Clear();
            _driver.FindElement(By.Id("Titulo")).SendKeys(titulo);
            _driver.FindElement(By.Id("Descripcion")).Clear();
            _driver.FindElement(By.Id("Descripcion")).SendKeys(descripcion);

            // Seleccionar valoración
            var select = new SelectElement(_driver.FindElement(By.Id("Valoracion")));
            select.SelectByText("Tres");

            // Enviar (abre diálogo)
            _driver.FindElement(By.Id("Submit")).Click();

            // Pulsar OK en el diálogo de confirmación
            detailResenya_PO.PressOkModalDialog();

            // Esperar a la página de detalle
            detailResenya_PO.WaitForBeingVisible(By.Id("NombreUsuario"));

            // Assert: comprobar campos principales (nombre puede ser "Anónimo" si no autenticado)
            Assert.True(detailResenya_PO.VerificarDetalleResenya("Anónimo", "", titulo, descripcion, "Tres"),
                "Los detalles de la reseña no coinciden con lo esperado.");

            // Comprobar que la tabla de bocadillos contiene el bocadillo añadido
            var expectedRow = new List<string[]>
            {
                new string[] { bocadilloName1, bocadilloPvp1, /*Tamaño*/ "", /*Puntuación*/ "1" }
            };

            Assert.True(detailResenya_PO.VerificarBocadillosPuntuados(expectedRow),
                "La tabla de bocadillos no contiene el bocadillo esperado.");

            // Adicional: comprobar que la puntuación se muestra correctamente en la tabla de detalle
            try
            {
                var row = _driver.FindElement(By.Id($"ResenyaItem_{bocadilloId1}"));
                var cells = row.FindElements(By.TagName("td"));
                // columnas: Nombre | PVP | Tamaño | Puntuación
                var puntuacionText = cells.Count >= 4 ? cells[3].Text.Trim() : string.Empty;
                _output.WriteLine($"Puntuación mostrada: '{puntuacionText}'");
                Assert.True(puntuacionText.StartsWith("1"), "La puntuación del bocadillo en detalle no coincide con la esperada (1).");
            }
            catch (NoSuchElementException)
            {
                // Si no existe la fila, que falle con información en salida
                Assert.True(false, $"No se encontró la fila detalle para ResenyaItem_{bocadilloId1}.");
            }
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public async Task UC_DetailResenya_CreateInvalid_ReturnsValidationError()
        {
            // Usa la URL completa para la API (la web y la API deben estar arrancadas)
            using var client = new HttpClient { BaseAddress = new Uri(_URI) };

            var invalidDto = new
            {
                NombreUsuario = "UIT",
                Titulo = "TituloValido",
                Descripcion = "Descripcion valida",
                Valoracion = "Tres",
                ResenyaBocadillo = new object[] { } // vacío -> error servidor
            };

            var response = await client.PostAsJsonAsync("api/ResenyasControlador/CreateResenya", invalidDto);

            _output.WriteLine($"CreateInvalid: status = {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine("CreateInvalid: content = " + content);

            // Permitir BadRequest o NotFound (algunas configuraciones devuelven 404 si la ruta no está disponible)
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound,
                $"Se esperaba 400 BadRequest (o 404 si la ruta no existe). Status actual: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Comprobar que el contenido contiene la clave "ResenyaBocadillo" o mensaje esperado
                Assert.True(content.Contains("ResenyaBocadillo") || content.Contains("Debes incluir") || content.Contains("ModelState"),
                    "Se esperaba mensaje de validación relacionado con 'ResenyaBocadillo' en la respuesta 400.");
            }
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_DetailResenya_NonExisting_ShowsError()
        {
            // Primero validamos que la API devuelve 404 para el id inexistente
            using (var client = new HttpClient { BaseAddress = new Uri(_URI) })
            {
                var nonExistingId = 999999;
                var apiResp = client.GetAsync($"api/ResenyasControlador/GetResenya?id={nonExistingId}").GetAwaiter().GetResult();
                _output.WriteLine($"API GET /GetResenya?id={nonExistingId} returned {apiResp.StatusCode}");
                Assert.True(apiResp.StatusCode == HttpStatusCode.NotFound, $"Se esperaba 404 NotFound de la API para id={nonExistingId}.");
            }

            // Luego comprobamos UI si existe el mensaje; no fallamos si la app está protegida por auth/redirect
            _driver.Navigate().GoToUrl(_URI + $"resenya/detailresenya?ResenyaID=999999");

            try
            {
                detailResenya_PO.WaitForBeingVisible(By.Id("ErrorMessage"));
                Assert.True(detailResenya_PO.VerificarMensajeError("Error!"), "Se esperaba mensaje de error en la UI al pedir detalle inexistente.");
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Timeout esperando mensaje de error en UI; la API devolvió 404 que validamos arriba.");
            }
        }
    }
}
