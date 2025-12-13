
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using System.Linq;
using System.Threading;

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

            // Obtener snapshot de textos de filas con reintentos para evitar StaleElementReferenceException
            var actualTexts = new List<string>();
            const int maxAttempts = 5;
            bool gotSnapshot = false;

            for (int attempt = 1; attempt <= maxAttempts && !gotSnapshot; attempt++)
            {
                try
                {
                    actualTexts.Clear();
                    var tbody = _driver.FindElement(tableOfBocadillos).FindElement(By.TagName("tbody"));
                    var actualRows = tbody.FindElements(By.TagName("tr"));

                    foreach (var r in actualRows)
                    {
                        // leer texto inmediatamente y almacenar en snapshot
                        // protegemos la lectura con try por si algún elemento se vuelve stale durante la iteración
                        try
                        {
                            actualTexts.Add(r.Text.Trim());
                        }
                        catch (StaleElementReferenceException)
                        {
                            // si un tr se vuelve stale, abortamos este intento y reintentamos desde cero
                            throw;
                        }
                    }

                    gotSnapshot = true;
                }
                catch (StaleElementReferenceException ex)
                {
                    _output.WriteLine($"StaleElementReferenceException al leer filas (intento {attempt}/{maxAttempts}): {ex.Message}");
                    Thread.Sleep(200); // pequeña espera antes de reintentar
                }
                catch (NoSuchElementException ex)
                {
                    _output.WriteLine($"NoSuchElementException al localizar tabla/filas (intento {attempt}/{maxAttempts}): {ex.Message}");
                    Thread.Sleep(200);
                }
            }

            if (!gotSnapshot)
            {
                _output.WriteLine("No se pudieron obtener filas estables de la tabla tras varios intentos.");
                return false;
            }

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
                // 1) elemento principal esperado
                var actualEl = _driver.FindElements(errorsShown).FirstOrDefault();
                if (actualEl != null && !string.IsNullOrWhiteSpace(actualEl.Text))
                {
                    _output.WriteLine($"actual Message shown:{actualEl.Text}");
                    return actualEl.Text.Contains(expectedMessage, StringComparison.InvariantCultureIgnoreCase);
                }

                // 2) fallbacks: alertas, validation summary, toasts, texto con clase text-danger
                var fallbackSelectors = new[] {
                    By.CssSelector(".alert, .alert-danger"),
                    By.CssSelector(".text-danger"),
                    By.CssSelector(".validation-summary-errors, .validation-summary-valid"),
                    By.CssSelector(".toast, .toast-body")
                };

                foreach (var sel in fallbackSelectors)
                {
                    var found = _driver.FindElements(sel);
                    foreach (var f in found)
                    {
                        if (!string.IsNullOrWhiteSpace(f.Text))
                        {
                            _output.WriteLine($"actual Message shown (fallback {sel}):{f.Text}");
                            if (f.Text.Contains(expectedMessage, StringComparison.InvariantCultureIgnoreCase))
                                return true;
                        }
                    }
                }

                // 3) buscar la palabra "Error" en el body como último recurso (registro para diagnóstico)
                try
                {
                    var bodyText = _driver.FindElement(By.TagName("body")).Text;
                    _output.WriteLine($"actual Message shown (body snippet): {(string.IsNullOrEmpty(bodyText) ? "<vacío>" : bodyText.Substring(0, Math.Min(300, bodyText.Length)))}");
                    if (!string.IsNullOrEmpty(expectedMessage))
                        return bodyText.Contains(expectedMessage, StringComparison.InvariantCultureIgnoreCase);
                }
                catch { /* ignore */ }

                _output.WriteLine("No se encontró el elemento de errores ni contenido conocido de error.");
                return false;
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