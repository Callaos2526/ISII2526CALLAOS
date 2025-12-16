using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class SelectBocadillosForPedido_PO : PageObject
    {
        private readonly By _selectTamanoBy = By.Id("selectTamano");
        private readonly By _inputTipoPanBy = By.Id("inputTipoPan");
        private readonly By _buttonBuscarBy = By.Id("buscarBocadillos");
        private readonly By _tableOfBocadillosBy = By.Id("TableOfBocadillos");
        private readonly By _errorsShownBy = By.Id("ErrorsShown");
        private readonly By _placePedidoButtonBy = By.Id("placePedidoButton");

        
        private IWebElement _selectTamano() => _driver.FindElement(_selectTamanoBy);
        private IWebElement _inputTipoPan() => _driver.FindElement(_inputTipoPanBy);
        private IWebElement _buttonBuscar() => _driver.FindElement(_buttonBuscarBy);
        private IWebElement _tableOfBocadillos() => _driver.FindElement(_tableOfBocadillosBy);
        private IWebElement _errorsShown() => _driver.FindElement(_errorsShownBy);
        private IWebElement _placePedidoButton() => _driver.FindElement(_placePedidoButtonBy);

        public SelectBocadillosForPedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        
        public void SearchBocadillos(string tamano, string tipoPan) //BUSCA BOCADILLOS segun el tamaño y tipo de pan que usu seleccione
        {
            WaitForBeingVisible(_selectTamanoBy); //espera a que este visible 

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)); //espera a que opciones esten cargadas en desplegable
            wait.Until(d =>
            {
                var sel = new SelectElement(d.FindElement(_selectTamanoBy));
                return sel.Options != null && sel.Options.Count > 0;
            });

            WaitForBeingClickable(_selectTamanoBy); //espera a que elem este listo para interactuar (clicar)
            var selectElement = new SelectElement(_selectTamano());

            if (string.IsNullOrWhiteSpace(tamano) || tamano == "All") //si tamaño= All o vacion selecciona todos
            {
                try { selectElement.SelectByText("Todos"); }
                catch (NoSuchElementException)
                {
                    if (selectElement.Options.Count > 0)
                        selectElement.SelectByIndex(0); //si no existe "Todos" selecciona la primera opcion
                }
            }
            else
            {
                //intenta seleccionar el tamaño indicado
                if (!TrySelect(selectElement, tamano))
                    _output?.WriteLine($"[SearchBocadillos] Opción de tamaño '{tamano}' no encontrada. Se deja sin filtro.");
            }

            WaitForBeingVisible(_inputTipoPanBy);
            var input = _inputTipoPan();
            input.Clear(); //borra valor actual del campo
            if (!string.IsNullOrWhiteSpace(tipoPan))
                input.SendKeys(tipoPan); //escribe el tipo de pan seleccionado

            WaitForBeingClickable(_buttonBuscarBy);
            _buttonBuscar().Click();

            //esperar a que tabla de bocadillos sea visible
            WaitForBeingVisible(_tableOfBocadillosBy);
        }


        //Metodo auxiliar para intentar seleccionar una opcion en un dropdown (desplegable)
        private bool TrySelect(SelectElement selectElement, string valueOrText)
        {
            try { selectElement.SelectByValue(valueOrText); return true; }
            catch (NoSuchElementException) { }

            try { selectElement.SelectByText(valueOrText); return true; }
            catch (NoSuchElementException) { }

            var partialText = selectElement.Options
                .FirstOrDefault(o => (o.Text ?? "").IndexOf(valueOrText, StringComparison.OrdinalIgnoreCase) >= 0);
            if (partialText != null)
            {
                selectElement.SelectByText(partialText.Text);
                return true;
            }

            var partialValue = selectElement.Options
                .FirstOrDefault(o => ((o.GetAttribute("value") ?? "")
                    .IndexOf(valueOrText, StringComparison.OrdinalIgnoreCase) >= 0));
            if (partialValue != null)
            {
                selectElement.SelectByValue(partialValue.GetAttribute("value"));
                return true;
            }

            return false;
        }

        
        //Metodo para comprobar que la tabla de bocadillos muestra los bocadillos esperados
        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
            => CheckBodyTable(expectedBocadillos, _tableOfBocadillosBy);

        
        //verifica si mensaje de error aparece en la pagina
        public bool CheckMessageError(string expectedMessage)
        {
            try
            {   
                //espera a que area de mensaje de error sea visible
                WaitForBeingVisible(_errorsShownBy);
                var actual = _errorsShown(); //tiene el mensaje de error que se esta mostrando en la UI
                _output?.WriteLine($"actual Message shown: {actual.Text}");
                return actual.Text.Contains(expectedMessage); //verifica si texto de mensaje de error contiene mensaje esperado
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }


        //Accion de agregar bocadillo al pedido
        public void AddBocadilloToPedido(string bocadilloId)
        {
            //localica boton de agregar bocaadillo (ID de boton incluye el bocadilloId, que identifica a cada bocadillo)
            var addBy = By.Id("bocadilloAPedir_" + bocadilloId);
            
            WaitForBeingClickable(addBy);
            _driver.FindElement(addBy).Click(); //hace click en boton para agregar bocadillo al pedido

            System.Threading.Thread.Sleep(500);
        }

        //Elimina bocadillo del pedido por su itemId
        public void RemoveBocadilloFromPedidoByItemId(string itemId)
        {
            var removeBy = By.Id("removeBocadillo_" + itemId);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElements(removeBy).Count > 0);

            WaitForBeingClickable(removeBy);
            //hace click en boton para eliminar el bocadillo del pedido
            _driver.FindElement(removeBy).Click();
            System.Threading.Thread.Sleep(500);
        }

        //verifica si pedido esta listo para realizarse 
        //          - si boton de realizar pedido no esta disponible devuelve TRUE, significa que pedido no esta disponible
        public bool PedidoNotAvailable()
        {
            try
            {
                var elems = _driver.FindElements(_placePedidoButtonBy); //localiza boton que permite realizar pedido
                if (elems == null || elems.Count == 0) return true;
                return !elems[0].Displayed; //elems[0].Displayed -> veridica si boton es visible en pagina
            }
            catch
            {
                return true;
            }
        }

        //Hace click en boton para proceder a crear el pedido, paso final en flujo de seleccion de bocadillos y creacion de pedido
        public void ProceedToCreatePedido()
        {
            WaitForBeingClickable(_placePedidoButtonBy);//hasta que crear pedido sea clicable 
            _placePedidoButton().Click();
        }

        //Obtiene el texto del carrito de compras ('Realizar Pedido), para verificar que se ve y por tanto esta presente en la pagina'
        public string GetCartButtonText()
        {
            try
            {
                return _placePedidoButton().Text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
