using OpenQA.Selenium;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.Resenya
{
    public class DetailResenya_PO : PageObject
    {
        public DetailResenya_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public bool CheckResenyaDetail(
            string nombreUsuario,
            DateTime fechaPublicacion,
            string titulo,
            string descripcion,
            string valoracion)
        {
            WaitForBeingVisible(By.Id("NombreUsuario"));

            bool result = true;

            result &= _driver.FindElement(By.Id("NombreUsuario")).Text.Contains(nombreUsuario);
            result &= _driver.FindElement(By.Id("Titulo")).Text.Contains(titulo);
            result &= _driver.FindElement(By.Id("Descripcion")).Text.Contains(descripcion);
            result &= _driver.FindElement(By.Id("Valoracion")).Text.Contains(valoracion);

            var actualDate = DateTime.Parse(
                _driver.FindElement(By.Id("FechaPublicacion")).Text
            );

            result &= ((actualDate - fechaPublicacion).Duration() < new TimeSpan(0, 1, 0));

            return result;
        }

        public bool CheckListOfResenyaBocadillos(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.Id("ResenyaBocadillos"));
        }

        
        public bool CheckErrorMessage(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(By.Id("ErrorMessage"));
                return _driver.FindElement(By.Id("ErrorMessage"))
                    .Text.Contains(expectedMessage, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return _driver.PageSource.Contains(expectedMessage);
            }
        }
    }
}
