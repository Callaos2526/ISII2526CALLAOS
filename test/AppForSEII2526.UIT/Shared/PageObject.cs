using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared
{
    public class PageObject
    {
        protected IWebDriver _driver;
        //this may be used whenever some result should be printed in Explorador de Pruebas
        protected readonly ITestOutputHelper _output;

        private By _modalTitle = By.ClassName("modal-title");
        private By _modalBody = By.ClassName("modal-body");
        private By _okModalDialog = By.Id("Button_DialogOK");


        protected PageObject(IWebDriver driver, ITestOutputHelper output)
        {
            _driver = driver;
            this._output = output;
        }


        public void InputDateInDatePicker(By datepicker, DateTime date)
        {
            //first we select the datepicker
            IWebElement webElement = _driver.FindElement(datepicker);

            var action = new OpenQA.Selenium.Interactions.Actions(_driver);
            webElement.Clear();
            webElement.Click();
            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Left).Perform();
            action.SendKeys(date.ToString("dd")).Perform();

            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Right).Perform();
            action.SendKeys(date.ToString("MM")).Perform();

            action.KeyDown(Keys.Right).Perform();
            action.KeyDown(Keys.Right).Perform();
            action.SendKeys(date.ToString("yyyy")).Perform();

        }


        public bool CheckBodyTable(List<string[]> expectedRows, By IdTable)
        {
            string expectedRow, actualRow;
            int i, j;
            bool result = true;
            WaitForBeingVisible(IdTable);

            IList<IWebElement> actualrows = _driver
                .FindElement(IdTable)
                .FindElement(By.TagName("tbody"))
                //.FindElements(By.XPath(".//tr"))
                .FindElements(By.TagName("tr"))//we obtain just the rows of the body of the table
                .ToList();

            if (actualrows.Count != expectedRows.Count)
            {
                _output.WriteLine($"Error: \n Expected number of rows:{expectedRows.Count} \n Actual number of rows:{actualrows.Count}");
                return false;
            }

            for (i = 0; i < expectedRows.Count; i++)
            {
                expectedRow = expectedRows[i][0];
                for (j = 1; j < expectedRows[i].Count(); j++)
                    expectedRow = expectedRow + " " + expectedRows[i][j];
                actualRow = actualrows
                    .Select(m => m.Text) //we return the text of the row
                    .ToList()[i];

                if (!actualRow.StartsWith(expectedRow))
                {
                    _output.WriteLine($"Error: \n \t expected row:{expectedRow} \n \t actual row:{actualRow}");
                    result = false;

                }
            }
            return result;

        }

        public bool CheckModalBodyText(string expectedBody, By modal)
        {
            //waiting for the message error to be shown
            WaitForBeingVisible(modal);
            var actualBody = _driver.FindElement(_modalBody).Text;
            return actualBody.Contains(expectedBody);
        }

        public bool CheckModalTitleText(string expectedTitle, By modal)
        {
            //waiting for the message error to be shown
            WaitForBeingVisible(modal);
            var actualTitle = _driver.FindElement(_modalTitle).Text;
            return actualTitle.Contains(expectedTitle);
        }

        // Nuevo: PressOkModalDialog más tolerante
        public void PressOkModalDialog()
        {
            // Try several heuristics to find and click a dialog OK button.
            // 1) try the explicit id
            var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            try
            {
                shortWait.Until(d => d.FindElement(_okModalDialog).Displayed);
                _driver.FindElement(_okModalDialog).Click();
                return;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"PressOkModalDialog: no se encontró botón por id 'Button_DialogOK': {ex.Message}");
            }

            // 2) try modal footer buttons (.modal .modal-footer button)
            try
            {
                shortWait.Until(d => d.FindElements(By.CssSelector(".modal .modal-footer button")).Count > 0);
                var footerButtons = _driver.FindElements(By.CssSelector(".modal .modal-footer button"));
                foreach (var b in footerButtons)
                {
                    if (b.Displayed && b.Enabled)
                    {
                        _output.WriteLine($"PressOkModalDialog: clicando botón en modal footer con texto '{b.Text}'.");
                        b.Click();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"PressOkModalDialog: no se encontró modal footer buttons: {ex.Message}");
            }

            // 3) try any visible button with common confirm texts
            var confirmTexts = new[] { "OK", "Aceptar", "Si", "Sí", "Confirm", "Confirmar", "Crear reseña", "Crear", "Yes" };
            try
            {
                shortWait.Until(d => d.FindElements(By.TagName("button")).Count > 0);
                var allButtons = _driver.FindElements(By.TagName("button"));
                foreach (var b in allButtons)
                {
                    if (!b.Displayed || !b.Enabled) continue;
                    var txt = (b.Text ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(txt)) continue;
                    if (confirmTexts.Any(ct => txt.Equals(ct, StringComparison.InvariantCultureIgnoreCase) || txt.Contains(ct, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _output.WriteLine($"PressOkModalDialog: clicando botón por texto '{txt}'.");
                        try { b.Click(); return; } catch { /* ignore and continue */ }
                    }
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"PressOkModalDialog: error buscando botones por texto: {ex.Message}");
            }

            // 4) fallback: try clicking the first visible button inside any element with role dialog
            try
            {
                var dialogs = _driver.FindElements(By.CssSelector("[role='dialog'], .modal"));
                foreach (var dialog in dialogs)
                {
                    var buttons = dialog.FindElements(By.TagName("button"));
                    foreach (var b in buttons)
                    {
                        if (b.Displayed && b.Enabled)
                        {
                            _output.WriteLine($"PressOkModalDialog: fallback click button '{b.Text}'.");
                            b.Click();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"PressOkModalDialog: fallback error: {ex.Message}");
            }

            // If we reach here, log and throw to fail fast (keeps original behavior)
            throw new NoSuchElementException("No se encontró ningún botón de confirmación del diálogo.");
        }


        public void PressOkModalDialog_Old()
        {
            //waiting for the message error to be shown
            WaitForBeingVisible(_okModalDialog);
            _driver.FindElement(_okModalDialog).Click();
        }



        public void WaitForBeingClickable(By IdElement)
        {
            //used whenever the webelement needs a delay for being clickable
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(IdElement));

        }

        public void WaitForBeingVisible(By IdElement)
        {
            //used whenever the webelement needs a delay for being clickable
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(IdElement));

        }

        public void WaitForBeingVisibleIgnoringExeptionTypes(By IdElement)
        {
            //used whenever the webelement needs a delay for being clickable
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 10, 0));


            wait.IgnoreExceptionTypes(typeof(NoSuchElementException),
                typeof(WebDriverTimeoutException),
                typeof(UnhandledAlertException),
                typeof(ElementClickInterceptedException));
            bool notFoundButton = true;
            while (notFoundButton)
            {
                try
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(IdElement));
                    notFoundButton = false;
                }
                catch (ElementClickInterceptedException ex)
                {
                    _output.WriteLine(ex.Message);
                }
            }
        }


        public void WaitForTextToBePresentInElement(By IdElement, string expectedText)
        {
            //used whenever the webelement needs a delay for being clickable
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));
            IWebElement element = _driver.FindElement(IdElement);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(element, expectedText));

        }


        //it wait for "seconds" till all the webelements of the page are loaded
        public void ImplicitWait(int seconds) =>
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
    }
}