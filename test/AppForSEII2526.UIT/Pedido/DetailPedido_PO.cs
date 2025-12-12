using OpenQA.Selenium;
using System.Threading;

namespace AppForSEII2526.UIT.PageObjects
{
    public class DetailPedido_PO : PageObject
    {
        private By _nombreClienteBy = By.Id("NameSurname");
        private By _metodoPagoBy = By.Id("PaymentMethod");
        private By _fechaPedidoBy = By.Id("FechaPedido");
        private By _bocadillosPedidoBy = By.Id("BocadillosPedido");
        private By _totalPriceBy = By.Id("TotalPrice");

        public DetailPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        // Métodos para interactuar con los elementos de la página

        public string GetNombreCliente()
        {
            return _driver.FindElement(_nombreClienteBy).Text;
        }

        public string GetMetodoPago()
        {
            return _driver.FindElement(_metodoPagoBy).Text;
        }

        public string GetFechaPedido()
        {
            return _driver.FindElement(_fechaPedidoBy).Text;
        }

        public string GetTotalPrice()
        {
            return _driver.FindElement(_totalPriceBy).Text;
        }

        public bool IsBocadillosVisible()
        {
            return _driver.FindElement(_bocadillosPedidoBy).Displayed;
        }

        public bool IsTotalPriceVisible()
        {
            return _driver.FindElement(_totalPriceBy).Displayed;
        }
    }
}
