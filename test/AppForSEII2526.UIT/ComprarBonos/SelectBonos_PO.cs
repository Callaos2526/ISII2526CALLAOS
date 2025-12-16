using OpenQA.Selenium;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace AppForSEII2526.UIT.ComprarBonos
{
    public class SelectBonos_PO : PageObject
    {
        public SelectBonos_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void SearchBonos(string nombre, string tipo)
        {
            WaitForBeingVisible(By.Id("inputNombre"));

            var nombreEl = _driver.FindElement(By.Id("inputNombre"));
            nombreEl.Clear();
            if (!string.IsNullOrEmpty(nombre))
                nombreEl.SendKeys(nombre);

            var tipoEl = _driver.FindElement(By.Id("inputTipo"));
            tipoEl.Clear();
            if (!string.IsNullOrEmpty(tipo))
                tipoEl.SendKeys(tipo);

            _driver.FindElement(By.Id("BuscarBonos")).Click();

            // Esperar a que la tabla esté lista tras la búsqueda
            WaitForBeingVisible(By.Id("TablaDeBonos"));
        }

        public bool CheckListOfBonos(List<string[]> expectedBonos)
        {
            WaitForBeingVisible(By.Id("TablaDeBonos"));
            return CheckBodyTable(expectedBonos, By.Id("TablaDeBonos"));
        }

        public bool CheckNoBonosFound()
        {
            // Esperar la tabla y comprobar que tbody no tiene filas
            WaitForBeingVisible(By.Id("TablaDeBonos"));
            var tbody = _driver.FindElement(By.Id("TablaDeBonos")).FindElement(By.TagName("tbody"));
            var rows = tbody.FindElements(By.TagName("tr"));
            if (rows.Count == 0)
            {
                // además comprobar que se muestra el mensaje inline o el modal
                try
                {
                    var msg = _driver.FindElement(By.Id("NoBonosMessage"));
                    if (msg.Displayed) return true;
                }
                catch (NoSuchElementException) { }

                try
                {
                    // modal visible
                    var modal = _driver.FindElement(By.Id("NoBonosModal"));
                    return modal.Displayed;
                }
                catch (NoSuchElementException) { }

                return true; // fallback: no filas -> sin resultados
            }
            return false;
        }

        public void AddBono(int bonoId)
        {
            try
            {
                WaitForBeingClickable(By.Id($"addBono_{bonoId}"));
                _driver.FindElement(By.Id($"addBono_{bonoId}")).Click();
            }
            catch
            {
                WaitForBeingClickable(By.Id($"BonoData_{bonoId}"));
                var row = _driver.FindElement(By.Id($"BonoData_{bonoId}"));
                var btn = row.FindElements(By.TagName("button")).FirstOrDefault();
                btn?.Click();
            }

            WaitForBeingVisible(By.Id("cartTotal"));
        }

        public void RemoveBonoFromCart(int bonoId)
        {
            WaitForBeingClickable(By.Id($"removeBono_{bonoId}"));
            _driver.FindElement(By.Id($"removeBono_{bonoId}")).Click();
            WaitForBeingVisibleIgnoringExeptionTypes(By.Id("cartTotal"));
        }

        public bool CheckCartTotal(string expectedTotal)
        {
            try
            {
                WaitForBeingVisible(By.Id("cartTotal"));
                var text = _driver.FindElement(By.Id("cartTotal")).Text;
                return text.Contains(expectedTotal);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsProceedButtonVisible()
        {
            try
            {
                return _driver.FindElement(By.Id("btnProcesarCompra")).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ProceedToCreatePurchase()
        {
            WaitForBeingClickable(By.Id("btnProcesarCompra"));
            _driver.FindElement(By.Id("btnProcesarCompra")).Click();
        }

        // Nuevo: cerrar modal de "no resultados" si aparece
        public void CloseNoBonosModalIfVisible()
        {
            try
            {
                var ok = _driver.FindElement(By.Id("NoBonosModal_Ok"));
                if (ok.Displayed)
                {
                    ok.Click();
                    WaitForBeingVisibleIgnoringExeptionTypes(By.Id("TablaDeBonos"));
                }
            }
            catch (NoSuchElementException) { }
        }

        public bool CheckErrorMessage(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(By.Id("ErrorsShown"));
                return _driver.FindElement(By.Id("ErrorsShown")).Text.Contains(expectedMessage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}