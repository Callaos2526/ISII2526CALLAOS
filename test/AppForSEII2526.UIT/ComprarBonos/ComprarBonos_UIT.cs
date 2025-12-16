using System;
using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using AppForSEII2526.UIT.ComprarBonos; // Page Object namespace
using AppForSEII2526.UIT.Shared;
using AppForSEII2526.UIT.PageObjects; // CreateCompraBono_PO

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

        // =====================================================
        // =============== PRUEBAS RELACIONADAS CON CREATE =====
        // =====================================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_Create_FlujoBasico_Ok()
        {
            Inicializar_SeleccionarBonos();

            // 1) Añadir bono y proceder a crear compra
            _selectPO.AddBono(BonoId1);
            _selectPO.ProceedToCreatePurchase();

            // 2) Crear PO de la página CreateCompra
            var createPO = new CrearCompraBono_PO(_driver, _output);
            createPO.WaitForBeingVisible(By.Id("SubmitCompra"));

            // 3) Rellenar datos mínimos y enviar
            createPO.FillInCompraInfo("Pedro", "Pérez", "Gómez", DateTime.Now, "Tarjeta");
            createPO.ClickSubmit();

            // 4) Confirmar diálogo/guardar (Popup de confirmación)
            try
            {
                createPO.PressSaveConfirmation(10);
            }
            catch
            {
                // si no aparece dialog, toleramos (dependiendo de la implementación)
            }

            // 5) Esperar detalle de la compra (página de detalle debe mostrar nombre)
            createPO.WaitForBeingVisible(By.Id("NameSurname"));
            var name = _driver.FindElement(By.Id("NameSurname")).Text;
            Assert.Contains("Pedro", name);
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_Create_Validation_ShowsErrors()
        {
            Inicializar_SeleccionarBonos();

            _selectPO.AddBono(BonoId1);
            _selectPO.ProceedToCreatePurchase();

            var createPO = new CrearCompraBono_PO(_driver, _output);
            createPO.WaitForBeingVisible(By.Id("SubmitCompra"));

            // Dejar el nombre vacío para forzar validación
            createPO.SetNombre(string.Empty);
            createPO.SetPrimerApellido(string.Empty); // forzar error adicional si procede
            createPO.SeleccionarMetodoPagoPorTexto("Tarjeta"); // dejar método para aislar nombre

            createPO.ClickSubmit();

            // Esperar que aparezcan errores en página
            Assert.True(createPO.CheckValidationError("Error! El nombre es obligatorio") || createPO.GetErrorsText().Length > 0,
                "Se esperaba que se mostrara un error de validación para campos obligatorios.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_ModifyFromCreate_ReturnsToSelectAndRetainsCart()
        {
            Inicializar_SeleccionarBonos();

            // Añadir bono y proceder
            _selectPO.AddBono(BonoId1);
            _selectPO.ProceedToCreatePurchase();

            var createPO = new CrearCompraBono_PO(_driver, _output);
            createPO.WaitForBeingVisible(By.Id("SubmitCompra"));

            // Pulsar modificar bonos -> debe volver a la pantalla de selección
            createPO.PressModificarBonos();

            // Esperar a la página de selección y comprobar que el carrito retiene el bono
            _selectPO.WaitForBeingVisible(By.Id("cartTotal"));
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), "El carrito debería mantener el bono seleccionado al volver desde Create.");
        }
    }
}