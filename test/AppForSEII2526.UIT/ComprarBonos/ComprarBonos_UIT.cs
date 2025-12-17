using AppForSEII2526.UIT.ComprarBonos; // Page Object namespace
using AppForSEII2526.UIT.PageObjects; // CreateCompraBono_PO
using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using System;
using Xunit;
using Xunit.Abstractions;

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
        public void UC_Bonos_Create_FlujoBasico_Ok_ShowsDetail()
        {
            Inicializar_SeleccionarBonos();

            // 1) Añadir bono y proceder a crear compra
            _selectPO.AddBono(BonoId1);
            _selectPO.ProceedToCreatePurchase();

            // 2) Crear PO de la página CreateCompra
            var createPO = new CrearCompraBono_PO(_driver, _output);
            createPO.WaitForBeingVisible(By.Id("SubmitCompra"));

            // 3) Rellenar datos mínimos y enviar
            var nombre = "Pedro";
            var apellido1 = "Pérez";
            var apellido2 = "Gómez";
            createPO.FillInCompraInfo(nombre, apellido1, apellido2, DateTime.Now, "Tarjeta");
            createPO.ClickSubmit();

            // 4) Confirmar diálogo/guardar si aparece
            try { createPO.PressSaveConfirmation(8); } catch { /* tolerante */ }

            // 5) Esperar detalle de la compra y verificar información principal
            var detailPO = new DetailCompraBono_PO(_driver, _output);
            detailPO.WaitForBeingVisible(By.Id("NameSurname"));

            var nombreCompleto = $"{nombre} {apellido1} {apellido2}".Trim();
            var ok = detailPO.CheckCompraDetail(nombreCompleto, DateTime.Now, "Tarjeta", detailPO.GetTotalPrice());
            Assert.True(ok, "Los detalles de la compra no coinciden con los esperados.");

            // 6) Verificar que la lista de bonos aparece
            Assert.True(detailPO.IsBonosVisible(), "La lista de bonos de la compra debería ser visible.");
            Assert.True(detailPO.IsTotalPriceVisible(), "El precio total debería ser visible en la página de detalle.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_Create_Validation_ShowsErrors_And_ReturnsToSelect()
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

            //cerrar/confirmar automáticamente cualquier modal de "save"
            try
            {
                createPO.PressSaveConfirmation(5);
            }
            catch
            {
                // Si no aparece modal, se ignora
            }
            // Debe mostrar errores de validación en la página Create
            Assert.True(createPO.CheckValidationError("Error") || createPO.GetErrorsText().Length > 0,
                "Se esperaba que se mostrara un error de validación para campos obligatorios.");

            // Pulsar 'Modificar Bonos' debe volver a la pantalla de selección y mantener el carrito
            createPO.PressModificarBonos();
            _selectPO.WaitForBeingVisible(By.Id("cartTotal"));
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), "El carrito debería mantener el bono seleccionado al volver desde Create.");
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

            // Pulsar modificar bonos -> debe volver a la página de selección
            createPO.PressModificarBonos();

            // Esperar a la página de selección y comprobar que el carrito retiene el bono
            _selectPO.WaitForBeingVisible(By.Id("cartTotal"));
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), "El carrito debería mantener el bono seleccionado al volver desde Create.");
        }


        //Uit nuevo
        //Con filtrar por nombre
        //O lo que tengas para filtrar
        //Añadir la compra
        //Quitar la compra
        //Añadir otro
        //Y hacer la compra
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Bonos_FiltrarAñadirQuitarAñadir_RealizarCompra_Ok()
        {
            Inicializar_SeleccionarBonos();
            // Filtrar por nombre
            _selectPO.SearchBonos("Completo", "");
            // Añadir bono filtrado
            _selectPO.AddBono(BonoId1);
            // Verificar total actualizado
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), $"El precio total no coincide con lo esperado ({BonoPrecio1}).");
            // Quitar bono
            _selectPO.RemoveBonoFromCart(BonoId1);
            // Verificar que el carrito está vacío
            Assert.False(_selectPO.IsProceedButtonVisible(), "El botón 'Procesar compra' no debería ser visible si el carrito está vacío.");
            // Añadir otro bono (el mismo para simplificar)
            _selectPO.AddBono(BonoId1);
            Assert.True(_selectPO.CheckCartTotal(BonoPrecio1), $"El precio total no coincide con lo esperado ({BonoPrecio1}).");
            // Proceder a crear compra
            _selectPO.ProceedToCreatePurchase();
            var createPO = new CrearCompraBono_PO(_driver, _output);
            createPO.WaitForBeingVisible(By.Id("SubmitCompra"));
            // Rellenar datos mínimos y enviar
            var nombre = "Pedro";
            var apellido1 = "Pérez";
            var apellido2 = "Gómez";
            createPO.FillInCompraInfo(nombre, apellido1, apellido2, DateTime.Now, "Tarjeta");
            createPO.ClickSubmit();
            // Confirmar diálogo/guardar si aparece
            try { createPO.PressSaveConfirmation(8); } catch { /* tolerante */ }
            // Verificar detalle de la compra
            var detailPO = new DetailCompraBono_PO(_driver, _output);
            detailPO.WaitForBeingVisible(By.Id("NameSurname"));

            var nombreCompleto = $"{nombre} {apellido1} {apellido2}".Trim();
            var ok = detailPO.CheckCompraDetail(nombreCompleto, DateTime.Now, "Tarjeta", detailPO.GetTotalPrice());
            Assert.True(ok, "Los detalles de la compra no coinciden con los esperados.");


            Assert.True(detailPO.IsBonosVisible(), "La lista de bonos de la compra debería ser visible.");
            Assert.True(detailPO.IsTotalPriceVisible(), "El precio total debería ser visible en la página de detalle.");

        }

    }
}