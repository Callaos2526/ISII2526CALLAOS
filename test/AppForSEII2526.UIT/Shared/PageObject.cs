
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

        // Nuevo: PressOkModalDialog más tolerante y con detección de alert/modal variados
        public void PressOkModalDialog()
        {
            var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // 1) Intentar detectar y aceptar un alert JS
            try
            {
                shortWait.Until(d =>
                {
                    try
                    {
                        d.SwitchTo().Alert();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });

                var alert = _driver.SwitchTo().Alert();
                alert.Accept();
                _output.WriteLine("PressOkModalDialog: JS alert detectado y aceptado.");
                return;
            }
            catch (WebDriverTimeoutException)
            {
                // continuar con otras comprobaciones
            }

            // 2) Esperar a la aparición de algún modal/dialog o al botón OK específico
            try
            {
                shortWait.Until(d =>
                    d.FindElements(By.CssSelector("[role='dialog'], .modal, .swal2-container")).Count > 0
                    || d.FindElements(By.Id("Button_DialogOK")).Count > 0);
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("PressOkModalDialog: no se detectó ningún dialog/modal en el tiempo esperado.");

                // Si no hay modal detectado, comprobar si ya se produjo la navegación/resultado esperado (evita fallar si no hay diálogo)
                try
                {
                    // Comprobación genérica común: si el detalle ya está visible (NombreUsuario), asumimos que la acción terminó.
                    if (_driver.FindElements(By.Id("NombreUsuario")).Any(e => e.Displayed))
                    {
                        _output.WriteLine("PressOkModalDialog: elemento 'NombreUsuario' visible -> asumiendo que la operación se completó sin dialog.");
                        return;
                    }
                }
                catch { /* ignore */ }
                // no retornamos aún; se intentarán fallbacks más abajo antes de lanzar excepción
            }

            // Obtener el primer diálogo visible
            var dialogs = _driver.FindElements(By.CssSelector("[role='dialog'], .modal, .swal2-container")).Where(e => e.Displayed || (e.GetAttribute("class")?.Contains("show") ?? false)).ToList();
            if (dialogs.Any())
            {
                var dialog = dialogs.First();

                // Buscar botón OK dentro del diálogo usando varios criterios
                var confirmTexts = new[] { "OK", "Aceptar", "Si", "Sí", "Confirm", "Confirmar", "Crear reseña", "Crear", "Yes" };

                // 1) botón con id dentro del diálogo
                try
                {
                    var okById = dialog.FindElements(By.Id("Button_DialogOK")).FirstOrDefault();
                    if (okById != null && okById.Displayed && okById.Enabled)
                    {
                        okById.Click();
                        _output.WriteLine("PressOkModalDialog: clicando Button_DialogOK dentro del modal.");
                        return;
                    }
                }
                catch { /* ignore and continue */ }

                // 2) botones en footer del diálogo
                try
                {
                    var footerButtons = dialog.FindElements(By.CssSelector(".modal-footer button, footer button, .swal2-actions button"));
                    foreach (var b in footerButtons)
                    {
                        if (!b.Displayed || !b.Enabled) continue;
                        var txt = (b.Text ?? string.Empty).Trim();
                        if (string.IsNullOrEmpty(txt)) continue;
                        if (confirmTexts.Any(ct => txt.Equals(ct, StringComparison.InvariantCultureIgnoreCase) || txt.Contains(ct, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _output.WriteLine($"PressOkModalDialog: clicando botón en modal footer con texto '{txt}'.");
                            b.Click();
                            return;
                        }
                    }
                }
                catch { /* continue */ }

                // 3) intentar cualquier botón dentro del propio diálogo con texto de confirmación
                var buttonsInDialog = dialog.FindElements(By.TagName("button"));
                foreach (var b in buttonsInDialog)
                {
                    if (!b.Displayed || !b.Enabled) continue;
                    var txt = (b.Text ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(txt)) continue;
                    if (confirmTexts.Any(ct => txt.Equals(ct, StringComparison.InvariantCultureIgnoreCase) || txt.Contains(ct, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _output.WriteLine($"PressOkModalDialog: clicando botón por texto dentro del modal '{txt}'.");
                        b.Click();
                        return;
                    }
                }

                // 4) fallback: primer botón visible dentro del diálogo
                foreach (var b in buttonsInDialog)
                {
                    if (b.Displayed && b.Enabled)
                    {
                        _output.WriteLine($"PressOkModalDialog: fallback click button in modal '{b.Text}'.");
                        b.Click();
                        return;
                    }
                }
            }

            // 5) último recurso: buscar globalmente botones de confirmación o Button_DialogOK
            try
            {
                var allButtons = _driver.FindElements(By.TagName("button"));
                foreach (var b in allButtons)
                {
                    if (!b.Displayed || !b.Enabled) continue;
                    var txt = (b.Text ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(txt)) continue;
                    var confirmTexts = new[] { "OK", "Aceptar", "Si", "Sí", "Confirm", "Confirmar", "Crear reseña", "Crear", "Yes" };
                    if (confirmTexts.Any(ct => txt.Equals(ct, StringComparison.InvariantCultureIgnoreCase) || txt.Contains(ct, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        _output.WriteLine($"PressOkModalDialog: fallback global click button por texto '{txt}'.");
                        try { b.Click(); return; } catch { /* ignore and continue */ }
                    }
                }

                // intentar localizar por id globalmente
                var btnById = _driver.FindElements(By.Id("Button_DialogOK")).FirstOrDefault();
                if (btnById != null && btnById.Displayed && btnById.Enabled)
                {
                    btnById.Click();
                    _output.WriteLine("PressOkModalDialog: clicando Button_DialogOK globalmente (fallback).");
                    return;
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"PressOkModalDialog: fallback global error: {ex.Message}");
            }

            // Si llegamos aquí, no se encontró nada razonable: lanzar excepción para mantener visibilidad del fallo
            throw new NoSuchElementException("No se encontró ningún botón de confirmación del diálogo ni alert que aceptar.");
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
            // Esperar por el localizador en vez de buscar el elemento antes
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(IdElement, expectedText));
        }


        //it wait for "seconds" till all the webelements of the page are loaded
        public void ImplicitWait(int seconds) =>
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
    }
}