using System;
using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using AppForSEII2526.UIT.ComprarBonos; // Page Object namespace
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Bonos
{
    public class ComprarBonos_UIT : UC_UIT
    {
        private readonly SelectBonos_PO _selectPO;

        // === DATOS DE PRUEBA (ajusta según seed DB) ===
        private const int BonoId1 = 1;
        private const string BonoPrecio1 = "10";

        public ComprarBonos_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectBonos_PO(_driver, _output);
        }

        private void Inicializar_SeleccionarBonos()
        {
            _driver.Navigate().GoToUrl(_URI + "ComprarBono/SelectBonoUI");

            // Esperar elementos clave
            _selectPO.WaitForBeingVisible(By.Id("BuscarBonos"));
            _selectPO.WaitForBeingVisible(By.Id("TablaDeBonos"));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_BusquedaDevuelveResultados()
        {
            Inicializar_SeleccionarBonos();

            // Buscar todos
            _selectPO.SearchBonos("", "");

            // Comprobar que la tabla tiene filas
            var rows = _driver
                .FindElement(By.Id("TablaDeBonos"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));

            Assert.True(rows.Count > 0, "Se esperaba al menos un bono en los resultados");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_AnadirYQuitarBono_ComprobarCarritoVacio()
        {
            Inicializar_SeleccionarBonos();

            // Añadir bono
            _selectPO.AddBono(BonoId1);

            // Verificar total actualizado
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), $"El precio total no coincide con lo esperado ({BonoPrecio1}).");

            // Quitar bono
            _selectPO.RemoveBonoFromCart(BonoId1);

            // Verificar que no se puede proceder
            Assert.False(_selectPO.IsProceedButtonVisible(), "El botón 'Procesar compra' no debería ser visible si el carrito está vacío.");
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Bono 5 bocadillos", "")]
        [InlineData("", "Normal")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_FiltradoNoLanzaExcepcion(string nombre, string tipo)
        {
            Inicializar_SeleccionarBonos();

            _selectPO.SearchBonos(nombre, tipo);

            // La tabla debe seguir presente
            Assert.NotNull(_driver.FindElement(By.Id("TablaDeBonos")));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_Select_NoResults_ShowsEmptyTable()
        {
            Inicializar_SeleccionarBonos();

            _selectPO.SearchBonos("NombreInventadoX", "TipoInventadoY");

            Assert.True(_selectPO.CheckNoBonosFound(), "La tabla debería estar vacía cuando no hay coincidencias.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_ProceedToCreate_NavigatesToCreate()
        {
            Inicializar_SeleccionarBonos();

            // Añadimos un bono para habilitar el botón y proceder
            _selectPO.AddBono(BonoId1);

            // Pulsar proceder (delegado al PageObject)
            _selectPO.ProceedToCreatePurchase();

            // Esperar navegación: buscar elemento clave de la página CreateCompra (ej. id SubmitCompra)
            _selectPO.WaitForBeingVisible(By.Id("SubmitCompra"));

            // Si el elemento existe, la navegación fue correcta
            Assert.True(_driver.FindElement(By.Id("SubmitCompra")).Displayed);
        }
    }
}