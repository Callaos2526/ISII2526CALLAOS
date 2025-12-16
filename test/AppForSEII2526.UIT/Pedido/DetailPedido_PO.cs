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

        //mira que detalles del pedido coincidan con los esperados
        public bool CheckPedidoDetail(string nombreCliente, DateTime fechaPedido, string metodoPago, string totalPrice)
        {
            WaitForBeingVisible(_totalPriceBy);

            bool result = true;
            // Comprobaciones de texto 
            result = result && _driver.FindElement(_nombreClienteBy).Text.Contains(nombreCliente); //mira que nombre cliente en la pagina contiene nombre esperado
            result = result && _driver.FindElement(_metodoPagoBy).Text.Contains(metodoPago); //mira que metodo de pago en pagina contiene valor esperado
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

        //verifica que la lista de bocadillos seleccionados en pedido sea la esperada
        public bool CheckListOfBocadillos(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _bocadillosPedidoBy);
        }


        //Metodos para obtener los detalles del pedido mostrados en la UI
        public string GetNombreCliente()
            => _driver.FindElement(_nombreClienteBy).Text;

        public string GetMetodoPago()
            => _driver.FindElement(_metodoPagoBy).Text;

        public string GetFechaPedido()
            => _driver.FindElement(_fechaPedidoBy).Text;

        public string GetTotalPrice()
            => _driver.FindElement(_totalPriceBy).Text;


        //metodos para comprobar la visibilidad de elementos en la UI (bocadillos y precio total)        
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
