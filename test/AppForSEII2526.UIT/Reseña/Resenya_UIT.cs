using Xunit;
using Xunit.Abstractions;
using OpenQA.Selenium;
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

        private void PrepareStateWithOneBocadillo()
        {
            NavigateToSelectResenya();
            selectPO.AddBocadillo(bocadilloId);
        }

        private void NavigateToCreateResenya()
        {
            _driver.Navigate().GoToUrl(_URI + "resenya/createresenya");
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
        public void UC_Resenya_Create_OK()
        {
            PrepareStateWithOneBocadillo();
            NavigateToCreateResenya();

            createPO.FillInResenyaInfo(
                usuario,
                "Reseña UIT",
                "Todo estaba muy bueno",
                valoracionTexto
            );

            createPO.PressCreateResenya();
            createPO.ConfirmDialog();

            Assert.True(
                _driver.Url.Contains("detailresenya"),
                "No se ha navegado al detalle tras crear la reseña"
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Create_ModifyBocadillos_NavigatesToSelect()
        {
            PrepareStateWithOneBocadillo();
            NavigateToCreateResenya();

            createPO.PressModifyBocadillos();

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
