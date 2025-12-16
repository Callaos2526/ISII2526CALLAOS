using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class DetailCompraBono_PO : PageObject
    {
        private readonly By _nombreClienteBy = By.Id("NameSurname");
        private readonly By _metodoPagoBy = By.Id("PaymentMethod");
        private readonly By _fechaCompraBy = By.Id("FechaCompra");
        private readonly By _bonosCompraBy = By.Id("BonosCompra");
        private readonly By _totalPriceBy = By.Id("TotalPrice");
        private readonly By _totalCantidadBy = By.Id("TotalCantidad");

        public DetailCompraBono_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        // Comprueba que los detalles de la compra de bonos coinciden con los esperados
        public bool CheckCompraDetail(string nombreCliente, DateTime fechaCompra, string metodoPago, string totalPrice)
        {
            WaitForBeingVisible(_totalPriceBy);

            bool result = true;
            // Comprobaciones de texto
            result = result && _driver.FindElement(_nombreClienteBy).Text.Contains(nombreCliente);
            result = result && _driver.FindElement(_metodoPagoBy).Text.Contains(metodoPago);
            result = result && _driver.FindElement(_totalPriceBy).Text.Contains(totalPrice);

            try
            {
                var fechaTexto = _driver.FindElement(_fechaCompraBy).Text;
                var actualFecha = DateTime.Parse(fechaTexto);

                // Si la fecha mostrada no incluye hora (time = 00:00:00) comparamos solo la parte Date
                if (actualFecha.TimeOfDay == TimeSpan.Zero)
                {
                    result = result && actualFecha.Date == fechaCompra.Date;
                }
                else
                {
                    // si incluye hora, permitimos una tolerancia razonable (1 minuto)
                    result = result && ((actualFecha - fechaCompra).Duration() < new TimeSpan(0, 1, 0));
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        // Verifica que la lista de bonos comprados en la compra sea la esperada
        public bool CheckListOfBonos(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _bonosCompraBy);
        }

        // Métodos para obtener los detalles mostrados en la UI
        public string GetNombreCliente()
            => _driver.FindElement(_nombreClienteBy).Text;

        public string GetMetodoPago()
            => _driver.FindElement(_metodoPagoBy).Text;

        public string GetFechaCompra()
            => _driver.FindElement(_fechaCompraBy).Text;

        public string GetTotalPrice()
            => _driver.FindElement(_totalPriceBy).Text;

        public string GetTotalCantidad()
            => _driver.FindElement(_totalCantidadBy).Text;

        // Métodos para comprobar visibilidad de elementos en la UI (tabla de bonos y precio total)        
        public bool IsBonosVisible()
        {
            try
            {
                return _driver.FindElement(_bonosCompraBy).Displayed;
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