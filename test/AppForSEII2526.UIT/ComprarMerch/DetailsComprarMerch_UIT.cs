using AppForSEII2526.UIT.ComprarMerch;
namespace AppForSEII2526.UIT.ComprarMerch
{
    public class DetailCompraMerch_UIT : UC_UIT
    {
        private readonly DetailCompraMerch_PO detailCompraMerch_PO;

        private const int compraId = 6;
        private const string nombre = "Pedro";          
        private const string apellido1 = "Apellido1";               
        private const string apellido2 = "";               
        private const string direccion = "Calle inventada1";
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
            // Arrange
            _driver.Navigate().GoToUrl(
                _URI + $"SeleccionarMerch/DetailsCompraMerch?CompraID={compraId}"
            );

            var nombreCompleto = $"{nombre} {apellido1} {apellido2}".Trim();

            // Act & Assert cabecera
            Assert.True(
                detailCompraMerch_PO.CheckCompraDetail(
                    nombreCompleto,
                    direccion,
                    metodoPago,
                    cantidadTotal
                )
            );

            var expectedItems = new List<string[]>
        {
            new string[] { "Camiseta", "4 €", "Camiseta", "1" }
        };


            Assert.True(detailCompraMerch_PO.CheckListaMerch(expectedItems));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Detail_InvalidId_ShowsError()
        {
            // Arrange
            _driver.Navigate().GoToUrl(
                _URI + "SeleccionarMerch/DetailsCompraMerch?CompraID=99999"
            );

            // Assert
            Assert.True(
                detailCompraMerch_PO.CheckErrorMessage("Error")
            );
        }
    }
}
