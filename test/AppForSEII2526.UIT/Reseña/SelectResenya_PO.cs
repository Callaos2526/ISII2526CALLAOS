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

            // Esperar hasta que se muestre la tabla de resultados o el contenedor de errores.
            try
            {
                // Primero intentamos esperar a la tabla (caso normal con resultados)
                WaitForBeingVisible(tableOfBocadillos);
            }
            catch (WebDriverTimeoutException)
            {
                // Si la tabla no aparece en el timeout, esperaremos al menos al contenedor de errores
                try
                {
                    WaitForBeingVisible(errorsShown);
                }
                catch (WebDriverTimeoutException)
                {
                    // si tampoco aparece nada, lo dejamos pasar: las pruebas deberán fallar con info de diagnóstico
                    _output.WriteLine("Ni tabla ni errores se hicieron visibles tras la búsqueda.");
                }
            }
        }

        // Nueva versión tolerante: busca una fila que contenga el nombre y el precio esperados.
        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {
            // Esperar la tabla visible antes de leer filas
            try
            {
                WaitForBeingVisible(tableOfBocadillos);
            }
            catch (WebDriverTimeoutException)
            {
                _output.WriteLine("Timeout esperando tabla en CheckListOfBocadillos.");
                return false;
            }

            // Obtener todas las filas de la tabla
            var tbody = _driver.FindElement(tableOfBocadillos).FindElement(By.TagName("tbody"));
            var actualRows = tbody.FindElements(By.TagName("tr"));

            // Convertir textos
            var actualTexts = new List<string>();
            foreach (var r in actualRows)
                actualTexts.Add(r.Text.Trim());

            // Para cada fila esperada, buscamos una fila real que contenga nombre y precio
            foreach (var expected in expectedBocadillos)
            {
                if (expected.Length == 0)
                {
                    _output.WriteLine("Expected row vacío en datos de prueba.");
                    return false;
                }

                var expectedName = expected[0].Trim();
                var expectedPrice = expected.Length > 0 ? expected[expected.Length - 1].Trim() : string.Empty;
                // Normalizar price (por si el test usa "6 €" o "6€")
                expectedPrice = expectedPrice.Replace(" ", "");

                bool found = false;
                foreach (var actual in actualTexts)
                {
                    var actualNormalized = actual.Replace(" ", "");
                    if (actual.Contains(expectedName, StringComparison.InvariantCultureIgnoreCase) &&
                        actualNormalized.Contains(expectedPrice))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _output.WriteLine($"No se encontró fila que contenga Nombre:'{expectedName}' y Precio:'{expectedPrice}'.");
                    _output.WriteLine("Filas reales encontradas:");
                    for (int i = 0; i < actualTexts.Count; i++)
                        _output.WriteLine($"  [{i}] {actualTexts[i]}");
                    return false;
                }
            }

            return true;
        }

        public void AddBocadilloToResenya(int bocadilloId)
        {
            var rowBy = By.Id("BocadilloData_" + bocadilloId);
            WaitForBeingClickable(rowBy);

            var row = _driver.FindElement(rowBy);
            var addButton = row.FindElement(By.TagName("button"));
            addButton.Click();

            // Esperar que la UI se actualice: botón para crear reseña o la lista de selección
            try
            {
                // Espera corta y robusta al botón de crear reseña
                WaitForBeingVisible(buttonCreateResenya);
            }
            catch (WebDriverTimeoutException)
            {
                // Si no aparece el botón, intentamos esperar a que la lista de selección tenga al menos una entrada.
                // Usamos la espera tolerante que ignora excepciones intermedias.
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