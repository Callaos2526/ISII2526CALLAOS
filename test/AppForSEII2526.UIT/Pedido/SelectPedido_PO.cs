using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    // Page Object model — estilo de la profesora
    public class SelectBocadillosForPedido_PO : PageObject
    {
        // locators (ids tal y como en la UI)
        By selectTamano = By.Id("selectTamano");
        By inputTipoPan = By.Id("inputTipoPan");
        By buttonBuscar = By.Id("buscarBocadillos");
        By tableOfBocadillos = By.Id("TableOfBocadillos"); // ajustar a TableOfBocadillos si es necesario
        By errorShown = By.Id("ErrorsShown");
        By placePedidoButton = By.Id("placePedidoButton");

        public SelectBocadillosForPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        // Busca bocadillos aplicando filtros (estilo profe)
        public void SearchBocadillos(string tamano, string tipoPan)
        {
            // wait for the webelement to be clickable
            WaitForBeingClickable(selectTamano);

            var select = new SelectElement(_driver.FindElement(selectTamano));
            if (string.IsNullOrEmpty(tamano) || tamano == "All")
                select.SelectByText("Todos");
            else
                select.SelectByText(tamano);

            _driver.FindElement(inputTipoPan).Clear();
            if (!string.IsNullOrEmpty(tipoPan))
                _driver.FindElement(inputTipoPan).SendKeys(tipoPan);

            _driver.FindElement(buttonBuscar).Click();

            // esperar a que la tabla esté visible / cargada
            WaitForBeingVisible(tableOfBocadillos);
        }

        // Comprueba la tabla usando el helper de la profesora
        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {
            return CheckBodyTable(expectedBocadillos, tableOfBocadillos);
        }

        // Comprueba que se muestre un mensaje de error concreto
        public bool CheckMessageError(string expectedMessage)
        {
            IWebElement actual = _driver.FindElement(errorShown);
            _output.WriteLine($"actual Message shown: {actual.Text}");
            return actual.Text.Contains(expectedMessage);
        }

        // Añadir bocadillo al carrito — espera a que el botón sea clickable y pulsa
        // El id en la UI es "bocadilloAPedir_{BocadilloID}" — pasar el sufijo (id) como string
        public void AddBocadilloToPedido(string bocadilloId)
        {
            var addBy = By.Id("bocadilloAPedir_" + bocadilloId);
            WaitForBeingClickable(addBy);
            _driver.FindElement(addBy).Click();
        }

        // Eliminar bocadillo del carrito — id "removeBocadillo_{Id}"
        public void RemoveBocadilloFromPedido(string bocadilloId)
        {
            var removeBy = By.Id("removeBocadillo_" + bocadilloId);
            WaitForBeingClickable(removeBy);
            _driver.FindElement(removeBy).Click();
        }

        // Comprueba si el botón de realizar pedido está oculto (no disponible)
        public bool PedidoNotAvailable()
        {
            // el botón no debe estar mostrado
            try
            {
                return _driver.FindElement(placePedidoButton).Displayed == false;
            }
            catch (NoSuchElementException)
            {
                // Si no existe, consideramos que no está disponible
                return true;
            }
        }

        // Navegar al formulario de crear pedido (igual que la profe)
        public void ProceedToCreatePedido()
        {
            WaitForBeingClickable(placePedidoButton);
            _driver.FindElement(placePedidoButton).Click();
        }
    }
}