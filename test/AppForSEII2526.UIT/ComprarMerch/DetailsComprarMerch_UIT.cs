using AppForSEII2526.UIT.ComprarMerch;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class DetailCompraMerch_UIT : UC_UIT
    {
        private readonly DetailCompraMerch_PO detailCompraMerch_PO;

        private const int compraId = 6;
        private const string nombre = "Miguel";
        private const string apellido1 = "Requena";
        private const string apellido2 = "";
        private const string direccion = "Calle prueba1";
        private const string metodoPago = "Tarjeta";
        private const int cantidadTotal = 1;

        public DetailCompraMerch_UIT(ITestOutputHelper output)
            : base(output)
        {
            detailCompraMerch_PO = new DetailCompraMerch_PO(_driver, _output);
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Detail_ShowsCorrectInformation()
        {
            // Arrange: navegar a la página de detalle
            _driver.Navigate().GoToUrl(_URI + $"SeleccionarMerch/DetailsCompraMerch?CompraID={compraId}");

            // Esperar ancla estable: nombre/apellidos en el detalle
            detailCompraMerch_PO.WaitForBeingVisible(By.Id("NombreApellidos"));

            var nombreCompleto = $"{nombre} {apellido1} {apellido2}".Trim();

            // Comprobar lista de items
            var expectedItems = new List<string[]>
            {
                new string[] { "Camiseta", "4 €", "Camiseta", "1" }
            };

            Assert.True(detailCompraMerch_PO.CheckListaMerch(expectedItems), "La lista de merchandising no coincide con la esperada.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Detail_InvalidId_ShowsError()
        {
            // Arrange: id no existente
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/DetailsCompraMerch?CompraID=99999");

            // Assert: la página debe mostrar un mensaje de error
            Assert.True(detailCompraMerch_PO.CheckErrorMessage("Error"), "Se esperaba un mensaje de error para id inválido.");
        }
    }
}
