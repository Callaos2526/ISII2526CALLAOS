using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using AppForSEII2526.UIT.PageObjects;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Pedido
{
    // Estilo similar al de la profesora: clase de UC que hereda de UC_UIT
    public class SelectPedido_UIT : UC_UIT
    {
        private SelectBocadillosForPedido_PO selectBocadillosForPedido_PO;

        // Datos (ajusta si prefieres otros valores reales de tu BD)
        private const string bocadilloId1 = "4";         // según dbo.ComprasBocadillos.data.sql
        private const string bocadilloName1 = "jamon";

        public SelectPedido_UIT(ITestOutputHelper output) : base(output)
        {
            selectBocadillosForPedido_PO = new SelectBocadillosForPedido_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            // Credenciales usadas en otros tests
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForPedido()
        {
            Precondition_perform_login();

            // Navegar directamente a la ruta del componente (más robusto que depender de un id de menú)
            _driver.Navigate().GoToUrl(_URI + "Pedido/SelectBocadillosParaPedir");

            // Esperar a que los filtros / tabla estén visibles
            selectBocadillosForPedido_PO.WaitForBeingVisible(By.Id("buscarBocadillos"));
            selectBocadillosForPedido_PO.WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_SearchReturnsResults()
        {
            // Arrange
            InitialStepsForPedido();

            // Act: buscar sin filtros (equivalente a "Todos")
            selectBocadillosForPedido_PO.SearchBocadillos("All", "");

            // Assert: la tabla debe contener al menos una fila
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
            // Arrange
            InitialStepsForPedido();

            // Act: añadir y luego eliminar el bocadillo con id conocido
            selectBocadillosForPedido_PO.AddBocadilloToPedido(bocadilloId1);
            selectBocadillosForPedido_PO.RemoveBocadilloFromPedido(bocadilloId1);

            // Assert: el botón de realizar pedido no debe estar disponible
            Assert.True(selectBocadillosForPedido_PO.PedidoNotAvailable());
        }

        [Theory]
        [InlineData("All", "")]
        [InlineData("Large", "Blanco")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Pedido_FilteringDoesNotThrow(string tamano, string tipoPan)
        {
            // Arrange
            InitialStepsForPedido();

            // Act
            selectBocadillosForPedido_PO.SearchBocadillos(tamano, tipoPan);

            // Assert: al menos que no lance excepciones y la tabla esté presente
            Assert.True(_driver.FindElement(By.Id("TableOfBocadillos")) != null);
        }
    }
}
