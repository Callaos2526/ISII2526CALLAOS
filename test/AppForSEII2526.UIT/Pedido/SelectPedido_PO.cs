using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class SelectBocadillosForPedido_PO : PageObject
    {
        private readonly By _selectTamanoBy = By.Id("selectTamano");
        private readonly By _inputTipoPanBy = By.Id("inputTipoPan");
        private readonly By _buttonBuscarBy = By.Id("buscarBocadillos");
        private readonly By _tableOfBocadillosBy = By.Id("TableOfBocadillos");
        private readonly By _errorsShownBy = By.Id("ErrorsShown");
        private readonly By _placePedidoButtonBy = By.Id("placePedidoButton");

        
        private IWebElement _selectTamano() => _driver.FindElement(_selectTamanoBy);
        private IWebElement _inputTipoPan() => _driver.FindElement(_inputTipoPanBy);
        private IWebElement _buttonBuscar() => _driver.FindElement(_buttonBuscarBy);
        private IWebElement _tableOfBocadillos() => _driver.FindElement(_tableOfBocadillosBy);
        private IWebElement _errorsShown() => _driver.FindElement(_errorsShownBy);
        private IWebElement _placePedidoButton() => _driver.FindElement(_placePedidoButtonBy);

        public SelectBocadillosForPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

       
        public void SearchBocadillos(string tamano, string tipoPan)
        {
            WaitForBeingVisible(_selectTamanoBy);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var sel = new SelectElement(d.FindElement(_selectTamanoBy));
                return sel.Options != null && sel.Options.Count > 0;
            });

            WaitForBeingClickable(_selectTamanoBy);
            var selectElement = new SelectElement(_selectTamano());

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
                    _output?.WriteLine($"[SearchBocadillos] Opción de tamaño '{tamano}' no encontrada. Se deja sin filtro.");
            }

            WaitForBeingVisible(_inputTipoPanBy);
            var input = _inputTipoPan();
            input.Clear();
            if (!string.IsNullOrWhiteSpace(tipoPan))
                input.SendKeys(tipoPan);

            WaitForBeingClickable(_buttonBuscarBy);
            _buttonBuscar().Click();

            WaitForBeingVisible(_tableOfBocadillosBy);
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
            => CheckBodyTable(expectedBocadillos, _tableOfBocadillosBy);

        
        public bool CheckMessageError(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(_errorsShownBy);
                var actual = _errorsShown();
                _output?.WriteLine($"actual Message shown: {actual.Text}");
                return actual.Text.Contains(expectedMessage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
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
                var elems = _driver.FindElements(_placePedidoButtonBy);
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
            WaitForBeingClickable(_placePedidoButtonBy);
            _placePedidoButton().Click();
        }

        
        public string GetCartButtonText()
        {
            try
            {
                return _placePedidoButton().Text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
