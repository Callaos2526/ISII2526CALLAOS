using System;
using System.Globalization;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class CreatePedido_PO : PageObject
    {
        private readonly By _nameBy = By.Id("Name");
        private readonly By _surname1By = By.Id("Surname1");
        private readonly By _surname2By = By.Id("Surname2");
        private readonly By _metodoPagoBy = By.Id("MetodoPago");
        private readonly By _submitBy = By.Id("SubmitPedido");
        private readonly By _modifyBocadillosBy = By.Id("ModifyBocadillos");
        private readonly By _tableOfPedidoItemsBy = By.Id("TableOfPedidoItems");
        private readonly By _errorsShownBy = By.Id("ErrorsShown");
        private readonly string _cantidadInputFormat = "cantidad_{0}";
    
        
        private IWebElement _name() => _driver.FindElement(_nameBy);
        private IWebElement _surname1() => _driver.FindElement(_surname1By);
        private IWebElement _surname2() => _driver.FindElement(_surname2By);
        private IWebElement _metodoPago() => _driver.FindElement(_metodoPagoBy);
        private IWebElement _submit() => _driver.FindElement(_submitBy);
        private IWebElement _modifyBocadillos() => _driver.FindElement(_modifyBocadillosBy);

        public CreatePedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        
        public void FillInPedidoInfo(string nombre, string apellido1, string apellido2, string metodoPago)
        {
            WaitForBeingVisible(_nameBy);
            _name().Clear();
            _name().SendKeys(nombre ?? string.Empty);

            _surname1().Clear();
            _surname1().SendKeys(apellido1 ?? string.Empty);

            _surname2().Clear();
            _surname2().SendKeys(apellido2 ?? string.Empty);

         
            var select = new SelectElement(_metodoPago());
            try
            {
                select.SelectByText(metodoPago);
            }
            catch (NoSuchElementException)
            {
                if (select.Options.Count > 0)
                    select.SelectByIndex(0);
            }
        }

       
        public void FillInCantidadItem(int itemId, int cantidad)
        {
            var id = string.Format(_cantidadInputFormat, itemId);
            var by = By.Id(id);
            WaitForBeingVisible(by);
            var input = _driver.FindElement(by);
            input.Clear();
            input.SendKeys(cantidad.ToString(CultureInfo.InvariantCulture));
        }

      
        public void PressRealizarPedido()
        {
            WaitForBeingClickable(_submitBy);
            _submit().Click();
        }

       
        public void PressModificarBocadillos()
        {
            WaitForBeingClickable(_modifyBocadillosBy);
            _modifyBocadillos().Click();
        }

       
        public bool CheckListOfPedidoItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _tableOfPedidoItemsBy);
        }

        
        public bool CheckValidationError(string expectedError)
        {
            try
            {
              
                var elems = _driver.FindElements(_errorsShownBy);
                if (elems.Count > 0)
                {
                    var txt = elems[0].Text ?? string.Empty;
                    _output?.WriteLine($"ErrorsShown: {txt}");
                    return txt.Contains(expectedError);
                }

               
                return _driver.PageSource.Contains(expectedError);
            }
            catch
            {
                return false;
            }
        }

        
        public string GetErrorsText()
        {
            try
            {
                WaitForBeingVisible(_errorsShownBy);
                return _driver.FindElement(_errorsShownBy).Text ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

       
        public bool TableOfPedidoItemsVisible()
        {
            try
            {
                WaitForBeingVisible(_tableOfPedidoItemsBy);
                return _driver.FindElement(_tableOfPedidoItemsBy).Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}
