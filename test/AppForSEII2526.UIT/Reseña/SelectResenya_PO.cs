using OpenQA.Selenium;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.Resenya
{
    public class SelectResenya_PO : PageObject
    {
        public SelectResenya_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void SearchBocadillos(string nombre, string pvp)
        {
            WaitForBeingVisible(By.Id("inputNombre"));

            var nombreEl = _driver.FindElement(By.Id("inputNombre"));
            nombreEl.Clear();
            if (!string.IsNullOrEmpty(nombre))
                nombreEl.SendKeys(nombre);

            var pvpEl = _driver.FindElement(By.Id("inputPvp"));
            pvpEl.Clear();
            if (!string.IsNullOrEmpty(pvp))
                pvpEl.SendKeys(pvp);

            _driver.FindElement(By.Id("searchBocadillos")).Click();

            // clave: esperar a que la tabla esté lista tras la búsqueda
            WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {
            WaitForBeingVisible(By.Id("TableOfBocadillos"));
            return CheckBodyTable(expectedBocadillos, By.Id("TableOfBocadillos"));
        }

        public void AddBocadillo(int bocadilloId)
        {
            WaitForBeingClickable(By.Id($"BocadilloData_{bocadilloId}"));

            _driver.FindElement(By.Id($"BocadilloData_{bocadilloId}"))
                   .FindElement(By.TagName("button"))
                   .Click();

            // clave: esperar a que aparezca el botón de crear reseña tras añadir
            WaitForBeingVisible(By.Id("goToCreateResenya"));
        }

        public bool CreateResenyaButtonVisible()
        {
            try
            {
                return _driver.FindElement(By.Id("goToCreateResenya")).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
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
