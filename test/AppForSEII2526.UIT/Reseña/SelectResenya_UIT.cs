
using AppForSEII2526.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Resenya
{
    public class SelectResenya_UIT : UC_UIT
    {
        private SelectResenya_PO selectResenya_PO;

        // Datos de prueba (ajusta si tu BD usa otros valores)
        private const int bocadilloId1 = 1;
        private const string bocadilloName1 = "Submarino";
        private const string bocadilloPvp1 = "6";

        private const int bocadilloId2 = 4;
        private const string bocadilloName2 = "jamon";
        private const string bocadilloPvp2 = "3";

        public SelectResenya_UIT(ITestOutputHelper output) : base(output)
        {
            selectResenya_PO = new SelectResenya_PO(_driver, _output);
        }

        private void InitialStepsForSelectResenya()
        {
            // Navegar directamente a la página de selección de bocadillos para reseña.
            _driver.Navigate().GoToUrl(_URI + "resenya/selectbocadillos");

            // Esperar a que la tabla de resultados esté visible antes de continuar.
            selectResenya_PO.WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        [Theory]
        [InlineData(bocadilloName1, "", bocadilloName1)]
        [InlineData("", bocadilloPvp2, bocadilloName2)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Filtering(string filterName, string filterPvp, string expectedName)
        {
            // Arrange
            InitialStepsForSelectResenya();

            // Construimos la fila esperada.
            var expected = new List<string[]>
            {
                new string[] {
                    expectedName,
                    /*Tamaño*/ "",
                    /*TipoPan*/ "",
                    /*Precio*/ (string.IsNullOrEmpty(filterPvp) ? bocadilloPvp1 + " €" : filterPvp + " €")
                }
            };

            // Act
            selectResenya_PO.SearchBocadillos(filterName, filterPvp);

            // Assert
            Assert.True(selectResenya_PO.CheckListOfBocadillos(expected));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_AddBocadillo_ShowsCreateButton()
        {
            // Arrange
            InitialStepsForSelectResenya();

            // Act - añadir un bocadillo a la reseña
            selectResenya_PO.AddBocadilloToResenya(bocadilloId1);

            // Assert - botón para ir a crear reseña visible
            Assert.True(selectResenya_PO.CreateResenyaButtonVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Search_NoResults_ShowsError()
        {
            // Arrange
            InitialStepsForSelectResenya();

            // Act - buscar algo que no existe para forzar que la API no devuelva resultados
            selectResenya_PO.SearchBocadillos("string_que_no_existe_12345", "");

            // Assert - la página debe mostrar algún mensaje de error
            Assert.True(selectResenya_PO.CheckMessageError("Error"), "Se esperaba un mensaje de error al buscar elementos inexistentes.");
        }
    }
}