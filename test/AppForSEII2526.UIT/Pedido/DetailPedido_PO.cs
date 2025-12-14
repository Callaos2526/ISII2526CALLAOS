using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class DetailPedido_PO : PageObject
    {
        private readonly By _nombreClienteBy = By.Id("NameSurname");
        private readonly By _metodoPagoBy = By.Id("PaymentMethod");
        private readonly By _fechaPedidoBy = By.Id("FechaPedido");
        private readonly By _bocadillosPedidoBy = By.Id("BocadillosPedido");
        private readonly By _totalPriceBy = By.Id("TotalPrice");

        public DetailPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public bool CheckPedidoDetail(string nombreCliente, DateTime fechaPedido, string metodoPago, string totalPrice)
        {
            WaitForBeingVisible(_totalPriceBy);

            bool result = true;

            result = result && _driver.FindElement(_nombreClienteBy).Text.Contains(nombreCliente);
            result = result && _driver.FindElement(_metodoPagoBy).Text.Contains(metodoPago);
            result = result && _driver.FindElement(_totalPriceBy).Text.Contains(totalPrice);

          
            try
            {
                var actualFecha = DateTime.Parse(_driver.FindElement(_fechaPedidoBy).Text);
                result = result && ((actualFecha - fechaPedido).Duration() < new TimeSpan(0, 1, 0));
            }
            catch
            {
                
                result = false;
            }

            return result;
        }

        public bool CheckListOfBocadillos(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _bocadillosPedidoBy);
        }


        public string GetNombreCliente()
            => _driver.FindElement(_nombreClienteBy).Text;

        public string GetMetodoPago()
            => _driver.FindElement(_metodoPagoBy).Text;

        public string GetFechaPedido()
            => _driver.FindElement(_fechaPedidoBy).Text;

        public string GetTotalPrice()
            => _driver.FindElement(_totalPriceBy).Text;

        public bool IsBocadillosVisible()
        {
            try
            {
                return _driver.FindElement(_bocadillosPedidoBy).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsTotalPriceVisible()
        {
            try
            {
                return _driver.FindElement(_totalPriceBy).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
