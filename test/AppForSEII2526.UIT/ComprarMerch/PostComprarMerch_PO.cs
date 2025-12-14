using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class CreateCompraMerch_PO : PageObject
    {
        private readonly By _nombreBy = By.Id("nombre");
        private readonly By _apellido1By = By.Id("apellido1");
        private readonly By _apellido2By = By.Id("apellido2");
        private readonly By _direccionBy = By.Id("Direccion_Envio");
        private readonly By _metodoPagoBy = By.Id("MetodoPago");
        private readonly By _submitBy = By.Id("SubmitPedido");
        private readonly By _modificarBy = By.Id("ModificarCompra");
        private readonly By _erroresBy = By.Id("ErrorsShown");

        public CreateCompraMerch_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public void FillInCompraInfo(string nombre, string apellido1, string apellido2, string direccionEnvio, string metodoPago)
        {
            WaitForBeingVisible(_nombreBy);

            var nombreEl = _driver.FindElement(_nombreBy);
            var ape1El = _driver.FindElement(_apellido1By);
            var ape2El = _driver.FindElement(_apellido2By);
            var dirEl = _driver.FindElement(_direccionBy);
            var metodoEl = _driver.FindElement(_metodoPagoBy);

            nombreEl.Clear(); nombreEl.SendKeys(nombre);
            ape1El.Clear(); ape1El.SendKeys(apellido1);
            ape2El.Clear(); ape2El.SendKeys(apellido2);
            dirEl.Clear(); dirEl.SendKeys(direccionEnvio);

            var selectMetodo = new SelectElement(metodoEl);
            selectMetodo.SelectByText(metodoPago);
        }

        public void PressRealizarCompra()
        {
            WaitForBeingClickable(_submitBy);
            _driver.FindElement(_submitBy).Click();
        }

        public void ConfirmDialog(bool accept)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            // Esperar a que aparezca algún elemento del modal (por ejemplo el botón "Save")
            var saveButton = wait.Until(
                ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(., 'Save')]"))
            );

            if (accept)
            {
                saveButton.Click();
            }
            else
            {
                var dontSave = _driver.FindElement(By.XPath("//button[contains(., \"Don't Save\")]"));
                dontSave.Click();
            }
        }

        public void PressModificarCompra()
        {
            WaitForBeingClickable(_modificarBy);
            _driver.FindElement(_modificarBy).Click();
        }

        public bool CheckTableOfMerch(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.Id("CompraMerch"));
        }

        public bool CheckValidationError(string expectedError)
        {
            // Esperar un poco a que Blazor renderice la validación
            System.Threading.Thread.Sleep(500);

            return _driver.PageSource.Contains(expectedError);
        }


        public bool SubmitButtonEnabled()
        {
            return _driver.FindElement(_submitBy).Enabled;
        }
    }
}
