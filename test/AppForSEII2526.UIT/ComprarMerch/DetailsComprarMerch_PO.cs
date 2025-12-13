using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class DetailCompraMerch_PO : PageObject
    {
        public DetailCompraMerch_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public bool CheckCompraDetail(
            string nombreCompleto,
            string direccionEnvio,
            string metodoPago,
            int cantidadTotal)
        {
            // Esperar a que se cargue el bloque de datos
            WaitForBeingVisible(By.Id("NombreApellidos"));

            bool result = true;

            result = result &&
                     _driver.FindElement(By.Id("NombreApellidos"))
                            .Text.Contains(nombreCompleto, StringComparison.InvariantCultureIgnoreCase);

            result = result &&
                     _driver.FindElement(By.Id("DireccionEnvio"))
                            .Text.Contains(direccionEnvio, StringComparison.InvariantCultureIgnoreCase);

            result = result &&
                     _driver.FindElement(By.Id("MetodoPago"))
                            .Text.Contains(metodoPago, StringComparison.InvariantCultureIgnoreCase);

            result = result &&
                     _driver.FindElement(By.Id("Cantidad"))
                            .Text.Contains(cantidadTotal.ToString());

            return result;
        }

        public bool CheckListaMerch(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.Id("CompraMerch"));
        }

        public bool CheckErrorMessage(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(By.Id("ErrorMessage"));
                return _driver.FindElement(By.Id("ErrorMessage"))
                              .Text.Contains(expectedMessage, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
