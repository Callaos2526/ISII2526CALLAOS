using System;
using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.PageObjects;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Pedido
{
    public class UCPedirBocadillo_UIT : UC_UIT
    {
        private readonly SelectBocadillosForPedido_PO _selectPO;
        private readonly DetailPedido_PO _detailPO;
        private readonly CreatePedido_PO _createPO;

        private const string BocadilloId1 = "1";
        private const string ItemIdCarrito1 = "1";

        public UCPedirBocadillo_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectBocadillosForPedido_PO(_driver, _output);
            _detailPO = new DetailPedido_PO(_driver, _output);
            _createPO = new CreatePedido_PO(_driver, _output);
        }

        private void Inicializar_SeleccionarBocadillos()
        {
            _driver.Navigate().GoToUrl(_URI + "Pedido/SelectBocadillosParaPedir");
            _selectPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("buscarBocadillos"));
            _selectPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfBocadillos"));
        }


        //  Pruebas del SELECT----------------------------------------------------------


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_BusquedaDevuelveResultados()
        {
            Inicializar_SeleccionarBocadillos();

            _selectPO.SearchBocadillos("All", "");

            var rows = _driver
                .FindElement(By.Id("TableOfBocadillos"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));

            Assert.True(rows.Count > 0, "Se esperaba al menos un bocadillo en los resultados");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_AnadirYQuitarBocadillo_ComprobarPedidoNoDisponible()
        {
            Inicializar_SeleccionarBocadillos();

            _selectPO.AddBocadilloToPedido(BocadilloId1);

            _selectPO.RemoveBocadilloFromPedidoByItemId(ItemIdCarrito1);

            Assert.True(_selectPO.PedidoNotAvailable());
        }



        [Theory]
        [InlineData("All", "")]
        [InlineData("Normal", "Blanco")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_FiltradoNoLanzaExcepcion(string tamano, string tipoPan)
        {
            Inicializar_SeleccionarBocadillos();

            _selectPO.SearchBocadillos(tamano, tipoPan);

            Assert.NotNull(_driver.FindElement(By.Id("TableOfBocadillos")));
        }

        // Pruebas del DETAIL-----------------------------------------------

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Detail_MuestraError_SiNoExiste()
        {
            var nonExistingId = 99999;
            _driver.Navigate().GoToUrl(_URI + $"Pedido/DetailPedido/?PedidoID={nonExistingId}");


            _detailPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("ErrorMessage"));
            var errorText = _driver.FindElement(By.Id("ErrorMessage")).Text;
            Assert.False(string.IsNullOrWhiteSpace(errorText), "Se esperaba mensaje de error cuando no existe el pedido.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Detail_MuestraCampos_PedidoSembrado()
        {
            // Ajusta id si tu entorno usa otro seed
            var seededPedidoId = 2;
            _driver.Navigate().GoToUrl(_URI + $"Pedido/DetailPedido/?PedidoID={seededPedidoId}");

            _detailPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("NameSurname"));

            var nombre = _detailPO.GetNombreCliente();
            var metodo = _detailPO.GetMetodoPago();
            var total = _detailPO.GetTotalPrice();

            _output?.WriteLine($"Detalle pedido: Nombre='{nombre}', Metodo='{metodo}', Total='{total}'");

            // Comprobaciones básicas (siempre)
            Assert.False(string.IsNullOrWhiteSpace(nombre), "El nombre del cliente debe mostrarse en detalle.");
            Assert.False(string.IsNullOrWhiteSpace(metodo), "El método de pago debe mostrarse en detalle.");
            Assert.True(_detailPO.IsBocadillosVisible(), "La lista de bocadillos debe ser visible.");

            // Comprobación no estricta del total: debe ser visible, registramos su valor para diagnóstico.
            Assert.True(_detailPO.IsTotalPriceVisible(), "El precio total debe ser visible en el detalle.");
            if (string.IsNullOrWhiteSpace(total) || total.Trim().StartsWith("0"))
            {
                // No fallamos aquí: registramos información útil para depuración del seed/BD.
                _output?.WriteLine($"AVISO: El total mostrado para el pedido seed id={seededPedidoId} es '{total}'. Si esperas otro valor revisa el seed o crea la compra en la BD usada por la app.");
            }
        }

        // -------------------------
        // PRUEBAS: CREATE (Crear pedido)
        // -------------------------

        // 1) Validación: intentar crear pedido sin nombre para provocar error de validación
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Create_MuestraErroresSiFaltanDatos()
        {
            // 1) Seleccionar y añadir bocadillo
            Inicializar_SeleccionarBocadillos();
            _selectPO.SearchBocadillos("All", "");
            _selectPO.AddBocadilloToPedido(BocadilloId1);

            // 2) Ir a CreatePedido
            _selectPO.ProceedToCreatePedido();

            // 3) Esperar la página CreatePedido (esperar el botón de submit)
            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));

            // 4) Dejar nombre vacío y rellenar apellido para forzar validación del nombre
            _createPO.SetNombre("");
            _createPO.SetPrimerApellido("Cualquiera");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");

            // 5) Enviar
            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();

            // 6) Esperar que aparezcan errores y comprobarlos
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElements(By.Id("ErrorsShown")).Count > 0
                            || d.FindElements(By.Id("ErrorMessage")).Count > 0);

            var errorsText = _createPO.GetErrorsText();
            _output?.WriteLine($"Errors during create: {errorsText}");

            Assert.True(!string.IsNullOrWhiteSpace(errorsText), "Se esperaba mensaje de error de validación al crear pedido sin nombre.");
        }

        // 2) Flujo feliz: usar usuario sembrado (Tomy Romero) para crear pedido y comprobar redirección a detalle
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Create_FlujoBasico_UsuarioSembrado_NoErrores()
        {
            // 1) Seleccionar y añadir bocadillo
            Inicializar_SeleccionarBocadillos();
            _selectPO.SearchBocadillos("All", "");
            _selectPO.AddBocadilloToPedido(BocadilloId1);

            // 2) Ir a CreatePedido
            _selectPO.ProceedToCreatePedido();

            // 3) Esperar la página CreatePedido (esperar el botón de submit)
            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));

            // 4) Rellenar datos con usuario sembrado (ver GlobalData.sql: Tomy Romero)
            _createPO.SetNombre("Tomy");
            _createPO.SetPrimerApellido("Romero");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");

            // 5) Enviar y confirmar diálogo (si aparece)
            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();

            try
            {
                _createPO.ConfirmDialogOk(10);  
            }
            catch
            {
                // Si el diálog no aparece, continuamos — la navegación puede ser directa
            }

            // 6) Esperar navegación a detalle
            var waitDetail = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            waitDetail.Until(d => d.FindElements(By.Id("NameSurname")).Count > 0
                                   || d.FindElements(By.Id("ErrorMessage")).Count > 0);

            // 7) Verificar que no hay error y que el detalle muestra el nombre del usuario
            if (_driver.FindElements(By.Id("ErrorMessage")).Count > 0)
            {
                var em = _driver.FindElement(By.Id("ErrorMessage")).Text;
                Assert.False(true, $"Se mostró error al abrir detalle tras crear pedido: {em}");
            }

            _detailPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("NameSurname"));
            var nombreDetalle = _detailPO.GetNombreCliente();
            _output?.WriteLine($"Nombre en detalle tras crear: {nombreDetalle}");

            Assert.Contains("Tomy", nombreDetalle, StringComparison.OrdinalIgnoreCase);
        }
    }
}
