using System.Collections.Generic;
using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class SelectComprarMerch_UIT : UC_UIT
    {
        private readonly SelectComprarMerch_PO selectMerch_PO;

        private const int merchId1 = 1;
        private const string merchNombre1 = "Camiseta";
        private const string merchPvp1 = "4";

        private const int merchId2 = 2;
        private const string merchNombre2 = "Pantalon";
        private const string merchPvp2 = "3";

        private const int merchId3 = 3;
        private const string merchNombre3 = "Chaqueta";
        private const string merchPvp3 = "50";

        public SelectComprarMerch_UIT(ITestOutputHelper output) : base(output)
        {
            selectMerch_PO = new SelectComprarMerch_PO(_driver, _output);
        }

        private void InitialStepsForSelectMerch()
        {
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/SelectCompraMerch");
            selectMerch_PO.WaitForBeingVisible(By.Id("TablaDeMerch"));
        }

        [Theory]
        [InlineData(merchNombre1, 50, merchNombre1, merchPvp1)]
        [InlineData(merchNombre2, 10, merchNombre2, merchPvp2)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Filtering(string filterTipo,
                                       int precioMax,
                                       string expectedName,
                                       string expectedPrice)
        {
            // Arrange
            InitialStepsForSelectMerch();
            var expected = new List<string[]>
            {
                new string[] { expectedName, expectedPrice }
            };

            // Act
            selectMerch_PO.SearchMerch(filterTipo, precioMax);

            // Assert
            Assert.True(selectMerch_PO.CheckListOfMerch(expected));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_AddMerch_ShowsPurchaseButton()
        {
            // Arrange
            InitialStepsForSelectMerch();
            selectMerch_PO.SearchMerch("", 50);

            // Act
            selectMerch_PO.AddMerchToCart(merchId1);

            // Assert
            Assert.True(selectMerch_PO.PurchaseButtonVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_AddAndRemoveMerch_RentingNotAvailable()
        {
            // Arrange
            InitialStepsForSelectMerch();
            selectMerch_PO.SearchMerch("", 50);

            // Act
            selectMerch_PO.AddMerchToCart(merchId1);

        }
    }
}
