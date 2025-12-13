using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace AppForSEII2526.UIT.Resenya
{
    public class SelectResenya_UIT : UC_UIT
    {
        private SelectResenya_PO selectResenya_PO;

        // Datos reales
        private const int bocadilloId = 1;
        private const string bocadilloName = "Submarino";

        public SelectResenya_UIT(ITestOutputHelper output)
            : base(output)
        {
            selectResenya_PO = new SelectResenya_PO(_driver, _output);
        }

        private void NavigateToSelectResenya()
        {
            _driver.Navigate().GoToUrl(_URI + "resenya/selectbocadillos");
            selectResenya_PO.WaitForBeingVisible(By.Id("TableOfBocadillos"));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_FilterByName()
        {
            // Arrange
            NavigateToSelectResenya();

            var expected = new List<string[]>
            {
                new string[] { "Submarino", "10", "Blanco", "6 €", "Add" }
            };

            // Act
            selectResenya_PO.SearchBocadillos(bocadilloName, "");

            // Assert
            Assert.True(selectResenya_PO.CheckListOfBocadillos(expected));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_AddBocadillo_ShowsCreateButton()
        {
            // Arrange
            NavigateToSelectResenya();

            // Act
            selectResenya_PO.AddBocadillo(bocadilloId);

            // Assert
            Assert.True(selectResenya_PO.CreateResenyaButtonVisible());
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Select_NoResults_ShowsError()
        {
            // Arrange
            NavigateToSelectResenya();

            // Act
            selectResenya_PO.SearchBocadillos("no_existe_12345", "");

            // Assert
            Assert.True(selectResenya_PO.CheckErrorMessage("Error"));
        }
    }
}
