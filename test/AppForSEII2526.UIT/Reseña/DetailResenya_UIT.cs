using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
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

            // Pulsar OK en el diálogo de confirmación (usar PO existente en lugar de instanciar PageObject protegido)
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
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_DetailResenya_NonExisting_ShowsError()
        {
            // Arrange: navegar a detalle con id que no existe
            var nonExistingId = 999999;
            _driver.Navigate().GoToUrl(_URI + $"resenya/detailresenya?ResenyaID={nonExistingId}");

            // Esperar al elemento de error
            try
            {
                detailResenya_PO.WaitForBeingVisible(By.Id("ErrorMessage"));
            }
            catch (WebDriverTimeoutException)
            {
                // si no aparece, fallará la aserción siguiente y dejaremos que la prueba muestre info
            }

            // Assert - debe mostrarse mensaje de error
            Assert.True(detailResenya_PO.VerificarMensajeError("Error!"),
                "Se esperaba mensaje de error al pedir detalle de reseña inexistente.");
        }
    }
}
