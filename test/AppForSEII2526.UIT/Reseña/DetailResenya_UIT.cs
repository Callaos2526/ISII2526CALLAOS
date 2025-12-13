using Xunit;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.Resenya
{
    public class DetailResenya_UIT : UC_UIT
    {
        private DetailResenya_PO detailResenya_PO;

        // DATOS REALES DE LA BD
        private const int resenyaId = 1;
        private const string usuario = "Miguel";
        private const string titulo = "R1";
        private const string descripcion = "Bien";
        private const string valoracion = "7";

        // Fecha aproximada (comparación tolerante ±1 min)
        private readonly DateTime fechaPublicacion =
            new DateTime(2024, 11, 11);

        public DetailResenya_UIT(ITestOutputHelper output)
            : base(output)
        {
            detailResenya_PO = new DetailResenya_PO(_driver, _output);
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Detail_ShowsCorrectInformation()
        {
            // Arrange
            _driver.Navigate().GoToUrl(
                _URI + $"resenya/detailresenya?ResenyaID={resenyaId}"
            );

            // Act & Assert
            Assert.True(
                detailResenya_PO.CheckResenyaDetail(
                    usuario,
                    fechaPublicacion,
                    titulo,
                    descripcion,
                    valoracion
                )
            );
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Resenya_Detail_InvalidId_ShowsError()
        {
            // Arrange
            _driver.Navigate().GoToUrl(
                _URI + "resenya/detailresenya?ResenyaID=99999"
            );

            // Assert
            Assert.True(
                detailResenya_PO.CheckErrorMessage("Error")
            );
        }
    }
}
