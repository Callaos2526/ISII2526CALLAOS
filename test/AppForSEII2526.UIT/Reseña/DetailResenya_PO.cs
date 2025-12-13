using OpenQA.Selenium;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.Resenya
{
    public class DetailResenya_PO : PageObject
    {
        public DetailResenya_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public bool CheckResenyaDetail(
            string nombreUsuario,
            DateTime fechaPublicacion,
            string titulo,
            string descripcion,
            string valoracion)
        {
            // Esperamos a que un elemento clave esté visible
            WaitForBeingVisible(By.Id("NombreUsuario"));

            bool result = true;

            result = result && _driver.FindElement(By.Id("NombreUsuario"))
                .Text.Contains(nombreUsuario);

            result = result && _driver.FindElement(By.Id("Titulo"))
                .Text.Contains(titulo);

            result = result && _driver.FindElement(By.Id("Descripcion"))
                .Text.Contains(descripcion);

            result = result && _driver.FindElement(By.Id("Valoracion"))
                .Text.Contains(valoracion);

            // Fecha: comparación tolerante (igual que hace la profe)
            var actualDate = DateTime.Parse(
                _driver.FindElement(By.Id("FechaPublicacion")).Text
            );

            result = result &&
                ((actualDate - fechaPublicacion).Duration() < new TimeSpan(0, 1, 0));

            return result;
        }

        public bool CheckListOfResenyaBocadillos(List<string[]> expectedItems)
        {
            // Reutilizamos el método común del PageObject base
            return CheckBodyTable(expectedItems, By.Id("ResenyaBocadillos"));
        }

        public bool CheckErrorMessage(string expectedMessage)
        {
            try
            {
                WaitForBeingVisible(By.Id("ErrorMessage"));
                return _driver.FindElement(By.Id("ErrorMessage"))
                    .Text.Contains(expectedMessage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
