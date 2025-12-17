using System.Collections.Generic;
using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class CreateCompraMerch_UIT : UC_UIT
    {
        private const int merchId1 = 1;
        private const string merchNombre1 = "Camiseta";
        private const string merchPvp1 = "4";

        private const int merchId2 = 2;
        private const string merchNombre2 = "Pantalon";
        private const string merchPvp2 = "3";
        private readonly SelectComprarMerch_PO select_PO;
        private readonly CreateCompraMerch_PO create_PO;

        public CreateCompraMerch_UIT(ITestOutputHelper output)
            : base(output)
        {
            select_PO = new SelectComprarMerch_PO(_driver, _output);
            create_PO = new CreateCompraMerch_PO(_driver, _output);
        }

        private void GoToCreateWithOneCamisetaInCart()
        {
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/SelectCompraMerch");
            select_PO.WaitForBeingVisible(By.Id("TablaDeMerch"));

            select_PO.SearchMerch("", 50);
            select_PO.AddMerchToCart(1); // id 1 = Camiseta

            _driver.FindElement(By.Id("purchaseMerchButton")).Click();
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Create_CorrectPurchase()
        {
            // Arrange
            GoToCreateWithOneCamisetaInCart();

            var expectedItems = new List<string[]>
            {
                new[] { "Camiseta", "4", "Camiseta", "1" }
            };
            Assert.True(create_PO.CheckTableOfMerch(expectedItems));

            // Act
            create_PO.FillInCompraInfo(
                "Miguel",
                "Requena",
                "",
                "Calle Prueba1",
                "Tarjeta"
            );

            create_PO.PressRealizarCompra();
            create_PO.ConfirmDialog(true);

            // Assert: esperar navegación a DetailsCompraMerch 
            var wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(5));
            wait.Until(d => d.Url.Contains("DetailsCompraMerch"));

            Assert.Contains("DetailsCompraMerch", _driver.Url);
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Create_InvalidData_ShowsError()
        {
            // Arrange: carrito con un producto
            GoToCreateWithOneCamisetaInCart();

            // Datos inválidos: nombre y apellido1 vacíos
            create_PO.FillInCompraInfo(
                "",
                "",
                "",
                "Calle Correcta 1",
                "PayPal"
            );

            // Act: intentar enviar el formulario
            create_PO.PressRealizarCompra();

            // Assert: se muestran errores de validación
            Assert.True(create_PO.CheckValidationError("The Nombre field is required."));

        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Merch_Create_ButtonDisabled_WhenCartEmpty()
        {
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/CreateCompraMerch");
            Assert.False(create_PO.SubmitButtonEnabled());
        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Examen()
        {
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/SelectCompraMerch");
            select_PO.WaitForBeingVisible(By.Id("TablaDeMerch"));

            select_PO.SearchMerch("Camiseta", 0);
            var expectedAfterFilter = new List<string[]> {
            new[] { merchNombre1, merchPvp1 }
            };
            Assert.True(select_PO.CheckListOfMerch(expectedAfterFilter));

            select_PO.AddMerchToCart(merchId1);
            Assert.True(select_PO.PurchaseButtonVisible());

            select_PO.SearchMerch("", 50);

            var removeButton = _driver.FindElement(By.Id("removeMerch_" + merchId1));
            removeButton.Click();
            Assert.False(select_PO.PurchaseButtonVisible());

            select_PO.SearchMerch("Pantalon", 0);
            var expectedPantalon = new List<string[]>{
            new[] { merchNombre2, merchPvp2 }
            };

            select_PO.AddMerchToCart(merchId2);
            Assert.True(select_PO.PurchaseButtonVisible());


            _driver.FindElement(By.Id("purchaseMerchButton")).Click();

            create_PO.FillInCompraInfo("Miguel", "Requena", "", "Calle Prueba1", "Tarjeta");
            create_PO.PressRealizarCompra();
            create_PO.ConfirmDialog(true);

            var wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(5));
            wait.Until(d => d.Url.Contains("DetailsCompraMerch"));
            Assert.Contains("DetailsCompraMerch", _driver.Url);
        }


    }
}
