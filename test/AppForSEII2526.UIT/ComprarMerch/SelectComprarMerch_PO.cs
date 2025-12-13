using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ComprarMerch
{
    internal class SelectComprarMerch_PO : PageObject
    {
        By inputTipo = By.Id("inputTipo");
        By inputPrecio = By.Id("inputPrecio");
        By buttonBuscarMerch = By.Id("BuscarMerch");
        By tablaMerch = By.Id("TablaDeMerch");
        By errorsShown = By.Id("ErrorsShown");
        By buttonPurchaseMerch = By.Id("purchaseMerchButton");

        public SelectComprarMerch_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public void SearchMerch(string tipo, int precioMaximo)
        {
            WaitForBeingClickable(inputTipo);
            var tipoEl = _driver.FindElement(inputTipo);
            tipoEl.Clear();
            if (!string.IsNullOrWhiteSpace(tipo))
                tipoEl.SendKeys(tipo);

            

            WaitForBeingClickable(buttonBuscarMerch);
            _driver.FindElement(buttonBuscarMerch).Click();

            try
            {
                WaitForBeingVisible(tablaMerch);
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    WaitForBeingVisible(errorsShown);
                }
                catch (WebDriverTimeoutException)
                {
                    _output.WriteLine("Ni tabla ni errores se hicieron visibles tras la búsqueda.");
                }
            }
        }

        public bool CheckListOfMerch(List<string[]> expectedMerch)
        {
            try
            {
                WaitForBeingVisible(tablaMerch);
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Timeout esperando tabla en CheckListOfMerch.");
                return false;
            }

            var tbody = _driver.FindElement(tablaMerch).FindElement(By.TagName("tbody"));
            var actualRows = tbody.FindElements(By.TagName("tr"));

            var actualTexts = new List<string>();
            foreach (var r in actualRows)
                actualTexts.Add(r.Text.Trim());

            foreach (var expected in expectedMerch)
            {
                if (expected.Length == 0)
                {
                    _output.WriteLine("Expected row vacío en datos de prueba.");
                    return false;
                }

                var expectedName = expected[0].Trim();
                var expectedPrice = expected[expected.Length - 1].Trim(); 
                expectedPrice = expectedPrice.Replace(" ", "");

                bool found = false;
                foreach (var actual in actualTexts)
                {
                    var actualNorm = actual.Replace(" ", "");
                    if (actual.Contains(expectedName, StringComparison.InvariantCultureIgnoreCase) &&
                        actualNorm.Contains(expectedPrice))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _output.WriteLine($"No se encontró fila con Nombre:'{expectedName}' y Precio:'{expectedPrice}'.");
                    _output.WriteLine("Filas reales:");
                    for (int i = 0; i < actualTexts.Count; i++)
                        _output.WriteLine($"  [{i}] {actualTexts[i]}");
                    return false;
                }
            }

            return true;
        }

        public void AddMerchToCart(int productoId)
        {
            var rowBy = By.Id("MerchData_" + productoId);
            WaitForBeingClickable(rowBy);

            var row = _driver.FindElement(rowBy);
            var addButton = row.FindElement(By.TagName("button"));
            addButton.Click();

            try
            {
                WaitForBeingVisible(buttonPurchaseMerch);
            }
            catch (WebDriverTimeoutException)
            {

                try
                {
                    WaitForBeingVisibleIgnoringExeptionTypes(By.CssSelector(".col-2 ul"));
                }
                catch (WebDriverTimeoutException)
                {
                    _output.WriteLine("Aviso: tras añadir bocadillo no apareció ni el botón ni la lista de selección en el tiempo de espera.");
                }
            }
        }

        public bool CheckMessageError(string expectedMessage)
        {
            try
            {
                var actual = _driver.FindElement(errorsShown);
                _output.WriteLine($"actual Message shown:{actual.Text}");
                return actual.Text.Contains(expectedMessage);
            }
            catch (NoSuchElementException)
            {
                _output.WriteLine("No se encontró el elemento de errores.");
                return false;
            }
        }

        public bool PurchaseButtonVisible()
        {
            try
            {
                return _driver.FindElement(buttonPurchaseMerch).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
