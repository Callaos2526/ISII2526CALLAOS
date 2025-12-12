using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
   
    public class SelectBocadillosForPedido_PO : PageObject
    {
       
        private readonly By selectTamano = By.Id("selectTamano");
        private readonly By inputTipoPan = By.Id("inputTipoPan");
        private readonly By buttonBuscar = By.Id("buscarBocadillos");
        private readonly By tableOfBocadillos = By.Id("TableOfBocadillos");
        private readonly By errorShown = By.Id("ErrorsShown");
        private readonly By placePedidoButton = By.Id("placePedidoButton");

        public SelectBocadillosForPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void SearchBocadillos(string tamano, string tipoPan)
        {
            
            WaitForBeingVisible(selectTamano);

           
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var sel = new SelectElement(d.FindElement(selectTamano));
                return sel.Options != null && sel.Options.Count > 0;
            });

            WaitForBeingClickable(selectTamano);

            var selectElement = new SelectElement(_driver.FindElement(selectTamano));

        
            if (string.IsNullOrWhiteSpace(tamano) || tamano == "All")
            {
                try { selectElement.SelectByText("Todos"); }
                catch (NoSuchElementException)
                {
                    if (selectElement.Options.Count > 0)
                        selectElement.SelectByIndex(0);
                }
            }
            else
            {
                if (!TrySelect(selectElement, tamano))
                    _output.WriteLine($"[SelectTamano] No encontrada opción '{tamano}'. Se deja sin filtro.");
            }

           
            WaitForBeingVisible(inputTipoPan);
            var input = _driver.FindElement(inputTipoPan);
            input.Clear();
            if (!string.IsNullOrWhiteSpace(tipoPan))
                input.SendKeys(tipoPan);

         
            WaitForBeingClickable(buttonBuscar);
            _driver.FindElement(buttonBuscar).Click();

          
            WaitForBeingVisible(tableOfBocadillos);
        }

        private bool TrySelect(SelectElement selectElement, string valueOrText)
        {
            try { selectElement.SelectByValue(valueOrText); return true; }
            catch (NoSuchElementException) { }

            try { selectElement.SelectByText(valueOrText); return true; }
            catch (NoSuchElementException) { }

            var partialText = selectElement.Options
                .FirstOrDefault(o => (o.Text ?? "").IndexOf(valueOrText, StringComparison.OrdinalIgnoreCase) >= 0);
            if (partialText != null)
            {
                selectElement.SelectByText(partialText.Text);
                return true;
            }

            var partialValue = selectElement.Options
                .FirstOrDefault(o => ((o.GetAttribute("value") ?? "")
                    .IndexOf(valueOrText, StringComparison.OrdinalIgnoreCase) >= 0));
            if (partialValue != null)
            {
                selectElement.SelectByValue(partialValue.GetAttribute("value"));
                return true;
            }

            return false;
        }

       
        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
            => CheckBodyTable(expectedBocadillos, tableOfBocadillos);

        public bool CheckMessageError(string expectedMessage)
        {
            IWebElement actual = _driver.FindElement(errorShown);
            _output.WriteLine($"actual Message shown: {actual.Text}");
            return actual.Text.Contains(expectedMessage);
        }

        
        public void AddBocadilloToPedido(string bocadilloId)
        {
            var addBy = By.Id("bocadilloAPedir_" + bocadilloId);
            WaitForBeingClickable(addBy);
            _driver.FindElement(addBy).Click();

           
            System.Threading.Thread.Sleep(500);
        }

       
        public void RemoveBocadilloFromPedidoByItemId(string itemId)
        {
            var removeBy = By.Id("removeBocadillo_" + itemId);

            
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElements(removeBy).Count > 0);

            WaitForBeingClickable(removeBy);
            _driver.FindElement(removeBy).Click();

            System.Threading.Thread.Sleep(500);
        }

      
        public bool PedidoNotAvailable()
        {
            try
            {
                var elems = _driver.FindElements(placePedidoButton);
                if (elems == null || elems.Count == 0) return true;
                return !elems[0].Displayed;
            }
            catch
            {
                return true;
            }
        }

        public void ProceedToCreatePedido()
        {
            WaitForBeingClickable(placePedidoButton);
            _driver.FindElement(placePedidoButton).Click();
        }
    }
}
