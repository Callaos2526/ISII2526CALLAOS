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

           
            Assert.False(string.IsNullOrWhiteSpace(nombre), "El nombre del cliente debe mostrarse en detalle.");
            Assert.False(string.IsNullOrWhiteSpace(metodo), "El método de pago debe mostrarse en detalle.");
            Assert.True(_detailPO.IsBocadillosVisible(), "La lista de bocadillos debe ser visible.");

           
            Assert.True(_detailPO.IsTotalPriceVisible(), "El precio total debe ser visible en el detalle.");
            if (string.IsNullOrWhiteSpace(total) || total.Trim().StartsWith("0"))
            {
               
                _output?.WriteLine($"AVISO: El total mostrado para el pedido seed id={seededPedidoId} es '{total}'. Si esperas otro valor revisa el seed o crea la compra en la BD usada por la app.");
            }
        }

        //Prueba del CREATE------------------------------------------------------------


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Create_MuestraErroresSiFaltanDatos()
        {
           
            Inicializar_SeleccionarBocadillos();
            _selectPO.SearchBocadillos("All", "");
            _selectPO.AddBocadilloToPedido(BocadilloId1);

           
            _selectPO.ProceedToCreatePedido();

           
            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));

          
            _createPO.SetNombre("");
            _createPO.SetPrimerApellido("Cualquiera");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");

          
            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();
            try
            {
                _createPO.PressSaveConfirmation(10);
            }
            catch
            {
                
            }

           
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(8));
            wait.Until(d => d.FindElements(By.Id("ErrorsShown")).Count > 0
                            || d.FindElements(By.Id("ErrorMessage")).Count > 0);

            var errorsText = _createPO.GetErrorsText();
            _output?.WriteLine($"Errors during create: {errorsText}");

            Assert.True(!string.IsNullOrWhiteSpace(errorsText), "Se esperaba mensaje de error de validación al crear pedido sin nombre.");
        }

      
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Create_FlujoBasico_UsuarioSembrado_NoErrores()
        {
            
            Inicializar_SeleccionarBocadillos();
            _selectPO.SearchBocadillos("All", "");
            _selectPO.AddBocadilloToPedido(BocadilloId1);

           
            _selectPO.ProceedToCreatePedido();

           
            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));

           
            _createPO.SetNombre("Tomy");
            _createPO.SetPrimerApellido("Romero");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");

           
            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();

            try
            {
                _createPO.PressSaveConfirmation(10);
            }
            catch
            {

            }

            
            var waitDetail = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            waitDetail.Until(d => d.FindElements(By.Id("NameSurname")).Count > 0
                                   || d.FindElements(By.Id("ErrorMessage")).Count > 0);

          
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
