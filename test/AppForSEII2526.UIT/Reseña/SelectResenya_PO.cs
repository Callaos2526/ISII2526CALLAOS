using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Resenya
{
    internal class SelectResenya_PO : PageObject
    {

        By inputNombre = By.Id("inputNombre");
        By inputPvp = By.Id("inputPvp");
        By buttonSearchBocadillos = By.Id("searchBocadillos");
        By tableOfBocadillos = By.Id("TableOfBocadillos");
        By errorsShown = By.Id("ErrorsShown");
        By buttonCreateResenya = By.Id("goToCreateResenya");

        public SelectResenya_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void SearchBocadillos(string nombre, string pvp)
        {
            // esperar y escribir nombre
            WaitForBeingClickable(inputNombre);
            var nombreEl = _driver.FindElement(inputNombre);
            nombreEl.Clear();
            if (!string.IsNullOrEmpty(nombre))
                nombreEl.SendKeys(nombre);

            // pvp (puede no existir si no renderiza)
            try
            {
                var pvpEl = _driver.FindElement(inputPvp);
                pvpEl.Clear();
                if (!string.IsNullOrEmpty(pvp))
                    pvpEl.SendKeys(pvp);
            }
            catch (NoSuchElementException) { }

            // click buscar
            WaitForBeingClickable(buttonSearchBocadillos);
            _driver.FindElement(buttonSearchBocadillos).Click();
        }

        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {
            return CheckBodyTable(expectedBocadillos, tableOfBocadillos);
        }

        public void AddBocadilloToResenya(int bocadilloId)
        {
            var rowBy = By.Id("BocadilloData_" + bocadilloId);
            WaitForBeingClickable(rowBy);

            var row = _driver.FindElement(rowBy);
            var addButton = row.FindElement(By.TagName("button"));
            addButton.Click();
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

        public bool CreateResenyaButtonVisible()
        {
            try
            {
                return _driver.FindElement(buttonCreateResenya).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}