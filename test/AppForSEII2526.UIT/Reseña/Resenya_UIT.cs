using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.Resenya
{
    public class UCResenya_UIT : UC_UIT
    {
        private SelectResenya_PO selectPO;
        private CreateResenya_PO createPO;
        private DetailResenya_PO detailPO;

        // === DATOS REALES DE TU BD ===
        private const int bocadilloId = 1;
        private const int resenyaIdExistente = 1;
        private const int resenyaIdInexistente = 99999;

        private const string usuario = "Miguel";
        private const string titulo = "R1";
        private const string descripcion = "Bien";
        private const string valoracionTexto = "Cuatro";
        private const string valoracionDetalle = "7";

        private readonly DateTime fechaPublicacion =
            new DateTime(2024, 11, 11);

        public UCResenya_UIT(ITestOutputHelper output)
            : base(output)
        {
            selectPO = new SelectResenya_PO(_driver, _output);
            createPO = new CreateResenya_PO(_driver, _output);
            detailPO = new DetailResenya_PO(_driver, _output);
        }

        // =====================================================
        // ================ MÉTODOS COMUNES ====================
        // =====================================================

        private void NavigateToSelectResenya()
        {
            _driver.Navigate().GoToUrl(_URI + "resenya/selectbocadillos");
            selectPO.WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        /// <summary>
        /// PREPARA ESTADO y entra a CREATE usando el botón real (NO GoToUrl),
        /// para no perder el Scoped state container en Blazor.
        /// </summary>
        private void PrepareStateAndNavigateToCreate_ByButton()
        {
            NavigateToSelectResenya();

            // añade bocadillo y espera que aparezca el botón
            selectPO.AddBocadillo(bocadilloId);
            selectPO.WaitForBeingVisible(By.Id("goToCreateResenya"));

            // IMPORTANTÍSIMO: entrar a Create como usuario, pulsando el botón
            _driver.FindElement(By.Id("goToCreateResenya")).Click();

            // ya estamos en create
            createPO.WaitForBeingVisible(By.Id("Submit"));
        }

        // =====================================================
        // ===================== SELECT =======================
        // =====================================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_FilterByName()
        {
            NavigateToSelectResenya();

            var expected = new List<string[]>
            {
                new string[] { "Submarino", "10", "Blanco", "6 €", "Add" }
            };

            selectPO.SearchBocadillos("Submarino", "");

            Assert.True(selectPO.CheckListOfBocadillos(expected));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_AddBocadillo_ShowsCreateButton()
        {
            NavigateToSelectResenya();

            selectPO.AddBocadillo(bocadilloId);

            Assert.True(selectPO.CreateResenyaButtonVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_NoResults_ShowsError()
        {
            NavigateToSelectResenya();

            selectPO.SearchBocadillos("no_existe_12345", "");

            Assert.True(selectPO.CheckErrorMessage("Error"));
        }

        // =====================================================
        // ===================== CREATE =======================
        // =====================================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void Sprint3Examen()
        {
            var createResenya_PO = new CreateResenya_PO(_driver, _output);
            var detailResenya_PO = new DetailResenya_PO(_driver, _output);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Ir a seleccionar bocadillos
            _driver.Navigate().GoToUrl(_URI + "resenya/selectbocadillos");
            selectPO.WaitForBeingVisible(By.Id("TableOfBocadillos"));

            // Añadir 
            selectPO.AddBocadillo(1);

            // Filtrar y añadir
            selectPO.SearchBocadillos("", "3");
            selectPO.AddBocadillo(4);

            var items = wait.Until(d => d.FindElements(By.CssSelector("li.list-group-item-action"))
            );
            items[0].Click(); // quitamos uno

            // Ir a crear reseña
            wait.Until(d => d.FindElement(By.Id("goToCreateResenya"))).Click();

            // Rellenar reseña
            createResenya_PO.FillInResenyaInfo(
            "Miguel",
            "Reseña Sprint 3",
            "Todo muy bien",
            "Cuatro"
            );

            // Crear reseña
            createResenya_PO.PressCreateResenya();

            // Pulso Save
            var saveBtn = wait.Until(d => {
                var el = d.FindElement(By.XPath("//button[normalize-space()='Save']"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            saveBtn.Click();

            // Esperar detalle
            wait.Until(d => d.Url.Contains("detailresenya"));

            Assert.True(
            _driver.FindElement(By.Id("ResenyaBocadillos")).Displayed,
            "No se ha mostrado el detalle de la reseña"
            );

            var expectedBocadillos = new List<string[]> {
new string[] { "jamon" }
};

            Assert.True(
            detailResenya_PO.CheckListOfResenyaBocadillos(expectedBocadillos),
            "Los bocadillos del detalle no son correctos"
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Create_OK()
        {
            // Entra a Create SIN perder estado
            PrepareStateAndNavigateToCreate_ByButton();

            createPO.FillInResenyaInfo(
                usuario,
                "Reseña UIT",
                "Todo estaba muy bueno",
                valoracionTexto
            );

            createPO.PressCreateResenya();

            // Esperar el modal y pulsar SAVE (el azul)
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Botón exactamente "Save" (no "Don't Save")
            var saveBtn = wait.Until(d =>
            {
                var el = d.FindElement(By.XPath("//button[normalize-space()='Save']"));
                return (el.Displayed && el.Enabled) ? el : null;
            });

            saveBtn.Click();

            // Esperar navegación al detalle
            wait.Until(d => d.Url.Contains("detailresenya"));

            Assert.True(
                _driver.Url.Contains("detailresenya"),
                "No se ha navegado al detalle tras crear la reseña"
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Create_ModifyBocadillos_NavigatesToSelect()
        {
            // Entra a Create SIN perder estado
            PrepareStateAndNavigateToCreate_ByButton();

            createPO.PressModifyBocadillos();

            
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.Contains("selectbocadillos"));

            Assert.True(_driver.Url.Contains("selectbocadillos"));
        }

        // =====================================================
        // ===================== DETAIL =======================
        // =====================================================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Detail_ShowsCorrectInformation()
        {
            _driver.Navigate().GoToUrl(
                _URI + $"resenya/detailresenya?ResenyaID={resenyaIdExistente}"
            );

            Assert.True(
                detailPO.CheckResenyaDetail(
                    usuario,
                    fechaPublicacion,
                    titulo,
                    descripcion,
                    valoracionDetalle
                ),
                "La información mostrada en el detalle no es correcta"
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Detail_InvalidId_ShowsError()
        {
            _driver.Navigate().GoToUrl(
                _URI + $"resenya/detailresenya?ResenyaID={resenyaIdInexistente}"
            );

            Assert.True(detailPO.CheckErrorMessage("Error"));
        }
    }
}
