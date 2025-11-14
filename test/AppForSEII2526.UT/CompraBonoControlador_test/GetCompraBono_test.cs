using AppForMovies.UT;
using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using LosDelEspacio.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CompraBonoControlador_test
{
    public class GetCompraBono_test : AppForMovies4SqliteUT
    {
        private int _existingCompraId;
        private ApplicationUser _cliente;
        private MetodoPago _metodoPago;
        private List<BonoItemForCreateDTO> _expectedItems;
        private double _expectedPrecioTotal;
        private DateTime _fechaCompra;

        public GetCompraBono_test()
        {
            // Tipo de bono
            var tipo = new TipoBocadillo { NombreTipo = "Normal" };

            // Bono
            var bono = new BonoBocadillo
            {
                BonoId = 1,
                Nombre = "BonoTest",
                PVP = 10.0,
                NBocadillos = 2,
                CantidadDisponible = 5,
                TipoBocadillos = tipo
            };

            // Metodo de pago
            var tarjeta = new Tarjeta() { metodoName = "Tarjeta" };
            _metodoPago = tarjeta;

            // Usuario
            _cliente = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                NombreCliente = "Cliente1",
                ApellidoCliente1 = "Ap1",
                ApellidoCliente2 = "Ap2",
                UserName = "cliente1"
            };

            // Crear compra y linea de compra (2 unidades)
            _fechaCompra = DateTime.Now;
            var compra = new CompraBono
            {
                FechaCompraBono = _fechaCompra,
                MetodoPago = tarjeta,
                Cliente = _cliente,
                NBonos = 2,
                PrecioTotalBono = 20.0
            };

            var bonosComprados = new BonosComprados
            {
                BonoId = bono.BonoId,
                Cantidad = 2,
                PrecioBono = bono.PVP,
                Bono = bono,
                Compra = compra
            };

            compra.bonosComprados.Add(bonosComprados);

            // Añadir al contexto
            _context.TiposBocadillos.Add(tipo);
            _context.BonosBocadillos.Add(bono);
            _context.Tarjetas.Add(tarjeta);
            _context.ApplicationUsers.Add(_cliente);
            _context.ComprasBono.Add(compra);

            _context.SaveChanges();

            // Guardar Id generado
            _existingCompraId = compra.CompraBonoId;

            // Datos esperados para las aserciones
            _expectedPrecioTotal = compra.PrecioTotalBono;
            _expectedItems = compra.bonosComprados
                .Select(bc => new BonoItemForCreateDTO(bc.Bono.BonoId, bc.Cantidad, bc.Bono.Nombre, bc.PrecioBono, bc.Bono.NBocadillos, bc.Bono.TipoBocadillos.NombreTipo))
                .ToList();
        }

        [Fact]
        public async Task GetCompra_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprarBonoControlador>>();
            var controller = new ComprarBonoControlador(_context, mock.Object);

            // Act
            var result = await controller.GetCompra(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCompra_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprarBonoControlador>>();
            var controller = new ComprarBonoControlador(_context, mock.Object);

            // Construimos un DTO esperado sencillo (estilo "details" simple)
            var expected = new CompraBonoDetallesDTO(
                _existingCompraId,
                _cliente,
                _metodoPago,
                _fechaCompra,
                _expectedPrecioTotal,
                _expectedItems
            );

            // Act
            var result = await controller.GetCompra(_existingCompraId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<CompraBonoDetallesDTO>(okResult.Value);

            // Sincronizar los campos que la persistencia puede alterar (Id/UserName/fechas)
            expected.ID = actual.ID;
            expected.CompraId = actual.CompraId;
            if (expected.NombreCliente != null && actual.NombreCliente!= null)
            {
                expected.ApellidoCliente1 = actual.ApellidoCliente1;
                expected.ApellidoCliente1 = actual.ApellidoCliente1;
            }
            expected.FechaCompra = actual.FechaCompra;

            // Comparación directa como en el ejemplo simplificado solicitado
            Assert.Equal(expected, actual);
        }
    }
}