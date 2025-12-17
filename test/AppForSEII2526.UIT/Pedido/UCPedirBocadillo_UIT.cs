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

        private const string BocadilloId1 = "1"; //
        private const string ItemIdCarrito1 = "1"; //articulo especifico del carrito de compra 
        private const string BocadilloId2 = "4";

        public UCPedirBocadillo_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectBocadillosForPedido_PO(_driver, _output);
            _detailPO = new DetailPedido_PO(_driver, _output);
            _createPO = new CreatePedido_PO(_driver, _output);
        }

        private void Inicializar_SeleccionarBocadillos() //navega a pagina de selccion de bocadillos y espera a que los elem sean visibles, antes de continuar
        {
            _driver.Navigate().GoToUrl(_URI + "Pedido/SelectBocadillosParaPedir");
            _selectPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("buscarBocadillos"));
            _selectPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfBocadillos"));
        }


        //  Pruebas del SELECT----------------------------------------------------------


        //verifica que Busqueda de Bocadillos devuelve resultados
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_BusquedaDevuelveResultados()
        {
            Inicializar_SeleccionarBocadillos(); //primero navega a pagina 

            _selectPO.SearchBocadillos("All", ""); //busqueda para obtener todos los bocadillos disponibles 

            var rows = _driver
                .FindElement(By.Id("TableOfBocadillos"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));
            //Se comprueba que al menos haya una fila en la tabla de bocadillos => se han devuelto resultados
            Assert.True(rows.Count > 0, "Se esperaba al menos un bocadillo en los resultados");
        }

        //Asegura que se puede añadir y quitar del pedido, y verifica que el pedido ya no esta disponible tras eliminar un bocadillo
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_AnadirYQuitarBocadillo_ComprobarPedidoNoDisponible()
        {
            Inicializar_SeleccionarBocadillos();

            _selectPO.AddBocadilloToPedido(BocadilloId1);

            _selectPO.RemoveBocadilloFromPedidoByItemId(ItemIdCarrito1);

            //Verifica que pedido ya no esta disponible 
            Assert.True(_selectPO.PedidoNotAvailable());
        }


        //Prueba parametrizada 
        //Comprueba que filtro de bocadillos funciona bien y no lanza excepcion
        [Theory]
        [InlineData("All", "")]
        [InlineData("Normal", "Blanco")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_FiltradoNoLanzaExcepcion(string tamano, string tipoPan)
        {
            Inicializar_SeleccionarBocadillos();

            _selectPO.SearchBocadillos(tamano, tipoPan); //realiza busqueda filtrada

            Assert.NotNull(_driver.FindElement(By.Id("TableOfBocadillos"))); //Asegura que tabla de bocadillos se sigue mostrando tras aplicar filtros
        }

        // Pruebas del DETAIL-----------------------------------------------

        //Verifica que muestra error si queremos acceder a un pedido que no existe
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

        //Verifica que campos del pedido se muestran correct. cuando se accede a un pedido que existe
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Detail_MuestraCampos_PedidoSembrado()
        {

            var seededPedidoId = 2;
            _driver.Navigate().GoToUrl(_URI + $"Pedido/DetailPedido/?PedidoID={seededPedidoId}");

            _detailPO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("NameSurname"));

            var nombre = _detailPO.GetNombreCliente();
            var metodo = _detailPO.GetMetodoPago();
            var total = _detailPO.GetTotalPrice();

            _output?.WriteLine($"Detalle pedido: Nombre='{nombre}', Metodo='{metodo}', Total='{total}'");

            //Asegura que nombre, metodo de pago, lista de bocadillos y precio total son visibles para pedido con ID 2
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


        //Verifica que se muestren errores si faltan datos al crear un pedido
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
            _createPO.SetPrimerApellido("Javier");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");

            //verifica que se muestren errores de validacion si no se ingresan datos validos 
            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();
            try
            {
                _createPO.PressSaveConfirmation(10);//le doy a boton de save para confirmar 
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

        //Verifica que flujo basico de creacion pedido funcione bien cuando se ingresan datos validos
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_Create_FlujoBasico_UsuarioSembrado_NoErrores()
        {

            Inicializar_SeleccionarBocadillos();
            _selectPO.SearchBocadillos("All", "");
            _selectPO.AddBocadilloToPedido(BocadilloId1);


            _selectPO.ProceedToCreatePedido();


            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));


            _createPO.SetNombre("Tomy"); // 
            _createPO.SetPrimerApellido("Romero");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");


            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();

            try
            {
                _createPO.PressSaveConfirmation(10); //confirma el guardado del pedido
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
        [Theory]
        [InlineData("pequeno", "")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Examen(string tamano, string tipoPan)
        {
            
        
            Inicializar_SeleccionarBocadillos();
            //añado
            _selectPO.AddBocadilloToPedido(BocadilloId1);
            //filtro
            _selectPO.SearchBocadillos(tamano,tipoPan);
            //añado otro
            _selectPO.AddBocadilloToPedido(BocadilloId2);
            //elimino primero
            _selectPO.RemoveBocadilloFromPedidoByItemId(ItemIdCarrito1);

            //flujo de la compra
            _selectPO.ProceedToCreatePedido();

            _createPO.WaitForBeingVisible(By.Id("SubmitPedido"));


            _createPO.SetNombre("Tomy"); // :*)
            _createPO.SetPrimerApellido("Romero");
            _createPO.SeleccionarMetodoPagoPorTexto("Tarjeta");


            Assert.True(_createPO.IsSubmitEnabled(), "El botón de enviar debe estar habilitado.");
            _createPO.ClickSubmit();

            try
            {
                _createPO.PressSaveConfirmation(10); //confirma el guardado del pedido
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
