using System;
using System.Globalization;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PageObjects
{
    public class CreatePedido_PO : PageObject
    {
        private readonly By _nameBy = By.Id("Name");
        private readonly By _surname1By = By.Id("Surname1");
        private readonly By _surname2By = By.Id("Surname2");
        private readonly By _metodoPagoBy = By.Id("MetodoPago");
        private readonly By _submitBy = By.Id("SubmitPedido"); //boton para enviar el pedido
        private readonly By _modifyBocadillosBy = By.Id("ModifyBocadillos");
        private readonly By _tableOfPedidoItemsBy = By.Id("TableOfPedidoItems");
        private readonly By _errorsShownBy = By.Id("ErrorsShown");
        private readonly string _cantidadInputFormat = "cantidad_{0}";

        private IWebElement _name() => _driver.FindElement(_nameBy);
        private IWebElement _surname1() => _driver.FindElement(_surname1By);
        private IWebElement _surname2() => _driver.FindElement(_surname2By);
        private IWebElement _metodoPago() => _driver.FindElement(_metodoPagoBy);
        private IWebElement _submit() => _driver.FindElement(_submitBy);
        private IWebElement _modifyBocadillos() => _driver.FindElement(_modifyBocadillosBy);

        public CreatePedido_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

      
        //Rellena los campos del formulario de creacion de pedido con los datos proporcionados
        public void FillInPedidoInfo(string nombre, string apellido1, string apellido2, string metodoPago)
        {
            WaitForBeingVisible(_nameBy);
            _name().Clear();
            _name().SendKeys(nombre ?? string.Empty);

            _surname1().Clear();
            _surname1().SendKeys(apellido1 ?? string.Empty);

            _surname2().Clear();
            _surname2().SendKeys(apellido2 ?? string.Empty);

            var select = new SelectElement(_metodoPago()); //selecciona metodo pago en desplegable
            try
            {
                select.SelectByText(metodoPago);
            }
            catch (NoSuchElementException)
            {
                if (select.Options.Count > 0)
                    select.SelectByIndex(0);
            }
        }


        //Rellenar campo de cantidad para un articulo especif., buscando el campo con su id
        public void FillInCantidadItem(int itemId, int cantidad) 
        {
            var id = string.Format(_cantidadInputFormat, itemId);
            var by = By.Id(id);
            WaitForBeingVisible(by);

            var input = _driver.FindElement(by);
            input.Clear();
            input.SendKeys(cantidad.ToString(CultureInfo.InvariantCulture));
        }

        //Hace click en bton Realizar Pedido, completando proceso de compra 
        public void PressRealizarPedido()
        {
            WaitForBeingClickable(_submitBy);
            _submit().Click();
        }

        //Hace click en bton Modificar Bocadillos
        public void PressModificarBocadillos()
        {
            WaitForBeingClickable(_modifyBocadillosBy);
            _modifyBocadillos().Click();
        }

        //Verifica que los articulos de la tabla de la web coinciden con los articulos esperados
        public bool CheckListOfPedidoItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _tableOfPedidoItemsBy);
        }


        //Comprueba si se muestra un error especifico en la pagina
        public bool CheckValidationError(string expectedError)
        {
            try
            {
                //busca elementos de error en la pagina
                var elems = _driver.FindElements(_errorsShownBy);
                if (elems.Count > 0)
                {
                    var txt = elems[0].Text ?? string.Empty;
                    _output?.WriteLine($"ErrorsShown: {txt}");
                    return txt.Contains(expectedError);//compara el mensaje de error mostrado con el mensaje esperado
                } 

                return _driver.PageSource.Contains(expectedError);
            }
            catch
            {
                return false;
            }
        }

        //Busca texto de los errores mostrados en la pagina si los hay
        public string GetErrorsText()
        {
            try
            {
                WaitForBeingVisible(_errorsShownBy);
                return _driver.FindElement(_errorsShownBy).Text ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool TableOfPedidoItemsVisible()
        {
            try
            {
                WaitForBeingVisible(_tableOfPedidoItemsBy);
                return _driver.FindElement(_tableOfPedidoItemsBy).Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void SetNombre(string nombre)
        {
            WaitForBeingVisible(_nameBy);
            _name().Clear();
            _name().SendKeys(nombre ?? string.Empty);
        }

        public void SetPrimerApellido(string apellido1)
        {
            WaitForBeingVisible(_surname1By);
            _surname1().Clear();
            _surname1().SendKeys(apellido1 ?? string.Empty);
        }

        public void SetSegundoApellido(string apellido2)
        {
            WaitForBeingVisible(_surname2By);
            _surname2().Clear();
            _surname2().SendKeys(apellido2 ?? string.Empty);
        }

        public void SeleccionarMetodoPagoPorTexto(string metodo)
        {
            WaitForBeingVisible(_metodoPagoBy);
            var select = new SelectElement(_metodoPago());
            try
            {
                select.SelectByText(metodo);
            }
            catch (NoSuchElementException)
            {
                if (select.Options.Count > 0)
                    select.SelectByIndex(0);
            }
        }


        
        public void ClickSubmit() => PressRealizarPedido();

        //Verifica si bon de enviar esta habilitado 
        public bool IsSubmitEnabled()
        {
            try { return _submit().Enabled; }
            catch { return false; }
        }

        public void ConfirmDialogOk(int timeoutSeconds = 10)
        {
            try
            {
               
                PressOkModalDialog();
            }
            catch
            {
                
                try
                {
                    var btns = _driver.FindElements(By.CssSelector(".modal-footer button"));
                    if (btns.Count > 0 && btns[0].Displayed && btns[0].Enabled)
                        btns[0].Click();
                }
                catch {  }
            }
        }

  
        //
        public void PressSaveConfirmation(int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));

           
            try
            {
                wait.Until(d => d.FindElements(By.CssSelector(".modal, [role='dialog'], .blazored-modal")).Count > 0);
            }
            catch {  }

            
            try
            {
                
                wait.Until(d => d.FindElements(By.Id("Button_DialogOK")).Count > 0);

                var okBtn = _driver.FindElement(By.Id("Button_DialogOK"));

               
                if (!okBtn.Displayed)
                {
                    try { ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", okBtn); } catch { }
                }

                
                try
                {
                    okBtn.Click();
                    return;
                }
                catch
                {
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", okBtn);
                    return;
                }
            }
            catch
            {
                
            }

            
            try
            {
                var texts = new[] { "Save", "Guardar", "Guardar pedido", "Aceptar" };
                foreach (var t in texts)
                {
                    var xpath = $"//div[contains(@class,'modal') or @role='dialog' or @aria-modal='true']//button[normalize-space()='{t}']";
                    try
                    {
                        var btn = wait.Until(d =>
                        {
                            var els = d.FindElements(By.XPath(xpath));
                            foreach (var e in els) if (e.Displayed && e.Enabled) return e;
                            return null;
                        });

                        if (btn != null)
                        {
                            try { btn.Click(); return; } catch { ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", btn); return; }
                        }
                    }
                    catch {  }
                }
            }
            catch { }

            try
            {
                var prim = _driver.FindElements(By.CssSelector("button.btn-primary"));
                foreach (var b in prim)
                {
                    if (b.Displayed && b.Enabled)
                    {
                        try { b.Click(); return; } catch { ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", b); return; }
                    }
                }
            }
            catch {  }

            
            throw new NoSuchElementException("No se pudo localizar ni pulsar el botón de confirmación del modal (Button_DialogOK / 'Save'). Comprueba en DevTools que el modal está visible y que el id 'Button_DialogOK' existe.");
        }
    }
}
