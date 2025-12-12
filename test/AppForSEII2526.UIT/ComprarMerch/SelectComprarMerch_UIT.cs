using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.ComprarMerch
{
    public class SelectComprarMerch_UIT : UC_UIT
    {
        private SelectComprarMerch_PO selectComprarMerch_PO;

        private const int productoId1 = 1;
        private const string productoNombre1 = "Camiseta";
        private const string productoTipo1 = "1"; 
        private const string productoStock1 = "5";
        private const string productoPrecio1 = "4";

        private const int productoId2 = 2;
        private const string productoNombre2 = "Pantalon";
        private const string productoTipo2 = "2"; 
        private const string productoStock2 = "10";
        private const string productoPrecio2 = "3";

        private const int productoId3 = 3;
        private const string productoNombre3 = "Chaqueta";
        private const string productoTipo3 = "3"; 
        private const string productoStock3 = "9";
        private const string productoPrecio3 = "50";

        public SelectComprarMerch_UIT(ITestOutputHelper output) : base(output)
        {
            selectComprarMerch_PO = new SelectComprarMerch_PO(_driver, _output);
        }

        /// Pasos iniciales para acceder a la página de comprar merch
        private void InitialStepsForComprarMerch()
        {
            // Navegar directamente a la página de selección de merch
            _driver.Navigate().GoToUrl(_URI + "SeleccionarMerch/SelectCompraMerch");

            // Esperar a que la página cargue (esperar al botón de búsqueda)
            selectComprarMerch_PO.WaitForBeingVisible(By.Id("BuscarMerch"));
        }

        /// Prueba: Filtrado por tipo "Camiseta" 
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Filtrado_Por_Tipo_Camiseta()
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { productoNombre1, productoTipo1, productoStock1, productoPrecio1 }
            };

            // Act
            selectComprarMerch_PO.SearchMerch("Camiseta", "");

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }

        /// Prueba: Filtrado por tipo "Pantalon" 
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Filtrado_Por_Tipo_Pantalon()
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { productoNombre2, productoTipo2, productoStock2, productoPrecio2 }
            };

            // Act
            selectComprarMerch_PO.SearchMerch("Pantalon", "");

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }

        /// Prueba: Filtrado por precio máximo 5 - debe devolver Camiseta (4€) y Pantalón (3€)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Filtrado_Por_Precio_Maximo_5()
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { productoNombre1, productoTipo1, productoStock1, productoPrecio1 },
                new string[] { productoNombre2, productoTipo2, productoStock2, productoPrecio2 }
            };

            // Act
            selectComprarMerch_PO.SearchMerch("", "5");

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }

        /// Prueba: Filtrado por precio máximo 10 - debe devolver todos los productos excepto Chaqueta (50€)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Filtrado_Por_Precio_Maximo_10()
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { productoNombre1, productoTipo1, productoStock1, productoPrecio1 },
                new string[] { productoNombre2, productoTipo2, productoStock2, productoPrecio2 }
            };

            // Act
            selectComprarMerch_PO.SearchMerch("", "10");

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }

        /// Prueba: Buscar todos los productos (sin filtros)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Sin_Filtros_Todos_Los_Productos()
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { productoNombre1, productoTipo1, productoStock1, productoPrecio1 },
                new string[] { productoNombre2, productoTipo2, productoStock2, productoPrecio2 },
                new string[] { productoNombre3, productoTipo3, productoStock3, productoPrecio3 }
            };

            // Act
            selectComprarMerch_PO.SearchMerch("", "");

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }

        /// Prueba: Añadir producto al carrito y verificar que el botón de compra esté disponible
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Añadir_Camiseta_Al_Carrito()
        {
            // Arrange
            InitialStepsForComprarMerch();
            selectComprarMerch_PO.SearchMerch("", ""); // Buscar todos los productos

            // Act
            selectComprarMerch_PO.AddMerchToCart(productoId1); // Añadir Camiseta

            // Assert
            Assert.True(selectComprarMerch_PO.PurchaseAvailable());
        }

        /// Prueba: Añadir y quitar producto del carrito - verificar que el botón de compra NO esté disponible
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Carrito_Vacio_Compra_No_Disponible()
        {
            // Arrange
            InitialStepsForComprarMerch();
            selectComprarMerch_PO.SearchMerch("", ""); // Buscar todos los productos

            // Act
            selectComprarMerch_PO.AddMerchToCart(productoId1); // Añadir Camiseta
            selectComprarMerch_PO.RemoveMerchFromCart(productoId1); // Quitar Camiseta

            // Assert
            Assert.True(selectComprarMerch_PO.PurchaseNotAvailable());
        }

        /// Prueba: Añadir múltiples productos al carrito
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Añadir_Multiples_Productos()
        {
            // Arrange
            InitialStepsForComprarMerch();
            selectComprarMerch_PO.SearchMerch("", ""); // Buscar todos los productos

            // Act
            selectComprarMerch_PO.AddMerchToCart(productoId1); // Añadir Camiseta
            selectComprarMerch_PO.AddMerchToCart(productoId2); // Añadir Pantalón

            // Assert
            Assert.True(selectComprarMerch_PO.PurchaseAvailable());
        }

        /// Prueba: Verificar precio total al añadir Camiseta (4€) y Pantalón (3€) = 7€
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Verificar_Precio_Total()
        {
            // Arrange
            InitialStepsForComprarMerch();
            selectComprarMerch_PO.SearchMerch("", ""); // Buscar todos los productos

            // Act
            selectComprarMerch_PO.AddMerchToCart(productoId1); // Añadir Camiseta (4€)
            selectComprarMerch_PO.AddMerchToCart(productoId2); // Añadir Pantalón (3€)

            // Assert - Total debe ser 7€
            Assert.True(selectComprarMerch_PO.CheckTotalPrice("7"));
        }

        /// Prueba: Añadir producto caro (Chaqueta 50€)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Añadir_Producto_Caro_Chaqueta()
        {
            // Arrange
            InitialStepsForComprarMerch();
            selectComprarMerch_PO.SearchMerch("Chaqueta", ""); // Buscar Chaqueta

            // Act
            selectComprarMerch_PO.AddMerchToCart(productoId3); // Añadir Chaqueta

            // Assert
            Assert.True(selectComprarMerch_PO.PurchaseAvailable());
            Assert.True(selectComprarMerch_PO.CheckTotalPrice("50"));
        }

        /// Prueba usando Theory con múltiples casos de filtrado
        [Theory]
        [InlineData("Camiseta", "", productoNombre1, productoTipo1, productoStock1, productoPrecio1)]
        [InlineData("Pantalon", "", productoNombre2, productoTipo2, productoStock2, productoPrecio2)]
        [InlineData("Chaqueta", "", productoNombre3, productoTipo3, productoStock3, productoPrecio3)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ComprarMerch_Theory_Filtrado_Por_Tipo(
            string filtroTipo, string filtroPrecio,
            string nombreEsperado, string tipoEsperado, string stockEsperado, string precioEsperado)
        {
            // Arrange
            InitialStepsForComprarMerch();
            var expectedMerch = new List<string[]>
            {
                new string[] { nombreEsperado, tipoEsperado, stockEsperado, precioEsperado }
            };

            // Act
            selectComprarMerch_PO.SearchMerch(filtroTipo, filtroPrecio);

            // Assert
            Assert.True(selectComprarMerch_PO.CheckListOfMerch(expectedMerch));
        }
    }
}
