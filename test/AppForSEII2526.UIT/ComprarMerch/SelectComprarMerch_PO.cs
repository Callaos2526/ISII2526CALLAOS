using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class SelectComprarMerch_PO : PageObject
    {
        // Localizadores de elementos
        private By inputTipo = By.Id("inputTipo");
        private By inputPrecio = By.Id("inputPrecio");
        private By buttonBuscarMerch = By.Id("BuscarMerch");
        private By tablaDeMerchBy = By.Id("TablaDeMerch");
        private By errorShownBy = By.Id("ErrorsShown");
        private By buttonPurchaseMerch = By.Id("purchaseMerchButton");

        public SelectComprarMerch_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        /// Busca merchandising por tipo y precio máximo
        public void SearchMerch(string tipo, string precioMaximo)
        {
            // Esperar a que el input de tipo sea clickeable
            WaitForBeingClickable(inputTipo);

            // Limpiar y escribir en el campo de tipo
            if (!string.IsNullOrEmpty(tipo))
            {
                _driver.FindElement(inputTipo).Clear();
                _driver.FindElement(inputTipo).SendKeys(tipo);
            }

            // Limpiar y escribir en el campo de precio
            if (!string.IsNullOrEmpty(precioMaximo))
            {
                _driver.FindElement(inputPrecio).Clear();
                _driver.FindElement(inputPrecio).SendKeys(precioMaximo);
            }

            // Hacer clic en el botón de búsqueda
            _driver.FindElement(buttonBuscarMerch).Click();
        }

        /// expextedMerch  -> Lista con arrays de: [Nombre, Tipo, Stock, Precio]
        public bool CheckListOfMerch(List<string[]> expectedMerch)
        {
            return CheckBodyTable(expectedMerch, tablaDeMerchBy);
        }

        /// Verifica que se muestre un mensaje de error específico
        public bool CheckMessageError(string errorMessage)
        {
            WaitForBeingVisible(errorShownBy);
            IWebElement actualErrorShown = _driver.FindElement(errorShownBy);
            _output.WriteLine($"Mensaje de error actual: {actualErrorShown.Text}");
            return actualErrorShown.Text.Contains(errorMessage);
        }

        /// Añade un producto al carrito de compra por su ID
        public void AddMerchToCart(int productoId)
        {
            By addButton = By.Id($"addMerch_{productoId}");
            WaitForBeingClickable(addButton);
            _driver.FindElement(addButton).Click();
        }

        /// Elimina un producto del carrito de compra por su ID
        public void RemoveMerchFromCart(int itemId)
        {
            By removeButton = By.Id($"removeMerch_{itemId}");
            WaitForBeingClickable(removeButton);
            _driver.FindElement(removeButton).Click();
        }

        /// Verifica si el botón de procesar compra NO está disponible (carrito vacío)
        public bool PurchaseNotAvailable()
        {
            try
            {
                // El botón está oculto cuando el carrito está vacío
                return _driver.FindElement(buttonPurchaseMerch).Displayed == false;
            }
            catch (NoSuchElementException)
            {
                // Si no se encuentra el elemento, también significa que no está disponible
                return true;
            }
        }

        /// Verifica si el botón de procesar compra SÍ está disponible
        public bool PurchaseAvailable()
        {
            try
            {
                WaitForBeingVisible(buttonPurchaseMerch);
                return _driver.FindElement(buttonPurchaseMerch).Displayed == true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// Hace clic en el botón de procesar compra
        public void ProceedToPurchase()
        {
            WaitForBeingClickable(buttonPurchaseMerch);
            _driver.FindElement(buttonPurchaseMerch).Click();
        }

        /// Verifica que un producto esté en la tabla de resultados
        public bool IsProductInTable(string nombreProducto)
        {
            WaitForBeingVisible(tablaDeMerchBy);
            var table = _driver.FindElement(tablaDeMerchBy);
            return table.Text.Contains(nombreProducto);
        }

        /// Verifica que el carrito muestre el precio total esperado
        public bool CheckTotalPrice(string expectedPrice)
        {
            // Buscar el elemento que muestra "Total: X €"
            var totalElements = _driver.FindElements(By.XPath("//*[contains(text(), 'Total:')]"));

            if (totalElements.Count > 0)
            {
                string totalText = totalElements[0].Text;
                _output.WriteLine($"Precio total mostrado: {totalText}");
                return totalText.Contains(expectedPrice);
            }

            return false;
        }

        /// Verifica que el mensaje "El carrito está vacío" esté visible
        public bool IsCartEmptyMessageVisible()
        {
            var emptyMessages = _driver.FindElements(By.XPath("//*[contains(text(), 'El carrito está vacío')]"));
            return emptyMessages.Count > 0 && emptyMessages[0].Displayed;
        }
    }
}
