using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Resenya
{
    public class CreateResenya_PO : PageObject
    {
        private By _nombreUsuarioBy = By.Id("NombreUsuario");
        private By _tituloBy = By.Id("Titulo");
        private By _descripcionBy = By.Id("Descripcion");
        private By _valoracionBy = By.Id("Valoracion");
        private By _submitBy = By.Id("Submit");

        public CreateResenya_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public void FillInResenyaInfo(string nombreUsuario, string titulo,
            string descripcion, string valoracion)
        {
            WaitForBeingVisible(_tituloBy);

            _driver.FindElement(_nombreUsuarioBy).Clear();
            _driver.FindElement(_nombreUsuarioBy).SendKeys(nombreUsuario);

            _driver.FindElement(_tituloBy).Clear();
            _driver.FindElement(_tituloBy).SendKeys(titulo);

            _driver.FindElement(_descripcionBy).Clear();
            _driver.FindElement(_descripcionBy).SendKeys(descripcion);

            new SelectElement(_driver.FindElement(_valoracionBy))
                .SelectByText(valoracion);
        }

        public void PressCreateResenya()
        {
            _driver.FindElement(_submitBy).Click();
        }

        
        public void ConfirmDialog()
        {
            WaitForBeingClickable(By.Id("DialogOKSaveDelete"));
            _driver.FindElement(By.Id("DialogOKSaveDelete")).Click();
        }

        
        public void PressModifyBocadillos()
        {
            WaitForBeingClickable(By.Id("ModifyBocadillos"));
            _driver.FindElement(By.Id("ModifyBocadillos")).Click();
        }
    }
}
