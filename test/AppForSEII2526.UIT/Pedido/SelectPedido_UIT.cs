using System;
using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.PageObjects;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Pedido
{
    public class SelectPedido_UIT : UC_UIT
    {
        private readonly SelectBocadillosForPedido_PO selectBocadillosForPedido_PO;

        private const string bocadilloId1 = "1";

    
        private const string itemIdCarrito1 = "1";

        public SelectPedido_UIT(ITestOutputHelper output) : base(output)
        {
            selectBocadillosForPedido_PO = new SelectBocadillosForPedido_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            
        }

        private void InitialStepsForPedido()
        {
            

            _driver.Navigate().GoToUrl(_URI + "Pedido/SelectBocadillosParaPedir");

          
            selectBocadillosForPedido_PO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("buscarBocadillos"));
            selectBocadillosForPedido_PO.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TableOfBocadillos"));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_SearchReturnsResults()
        {
            InitialStepsForPedido();

            selectBocadillosForPedido_PO.SearchBocadillos("All", "");

            var rows = _driver
                .FindElement(By.Id("TableOfBocadillos"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));

            Assert.True(rows.Count > 0, "Se esperaba al menos un bocadillo en los resultados");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_AddAndRemoveBocadillo_CheckPedidoNotAvailable()
        {
            InitialStepsForPedido();

            selectBocadillosForPedido_PO.AddBocadilloToPedido(bocadilloId1);

           
            selectBocadillosForPedido_PO.RemoveBocadilloFromPedidoByItemId(itemIdCarrito1);

            Assert.True(selectBocadillosForPedido_PO.PedidoNotAvailable());
        }

        [Theory]
        [InlineData("All", "")]
        [InlineData("Large", "Blanco")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_FilteringDoesNotThrow(string tamano, string tipoPan)
        {
            InitialStepsForPedido();

            selectBocadillosForPedido_PO.SearchBocadillos(tamano, tipoPan);

            Assert.NotNull(_driver.FindElement(By.Id("TableOfBocadillos")));
        }
    }
}
