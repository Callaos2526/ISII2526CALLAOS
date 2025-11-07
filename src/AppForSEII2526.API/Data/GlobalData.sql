INSERT INTO [dbo].[AspNetUsers] ([Id], [NombreCliente], [ApellidoCliente1], [ApellidoCliente2], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'1', N'Tomy', N'Romero', N'Cejudo', N'Tomy', N'Tomy', N'tomas.romero1@alu.uclm.es', N'tomas.romero1@alu.ucml.es', 1, N'213131', N'232', N'23232', N'434123432', 1, 0, N'11/10/2025 0:00:00 +02:00', 0, 22)
INSERT INTO [dbo].[AspNetUsers] ([Id], [NombreCliente], [ApellidoCliente1], [ApellidoCliente2], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'49007c32-3268-4d9d-89c0-bd1daf9b8903', N'Miguel', N'Requena', N'po', N'Miguel', NULL, NULL, NULL, 0, NULL, N'3831bbb3-c4a3-46b2-b34c-d0d4c722bf1f', N'b3cb1f88-251b-4c3a-b6ac-b82d729d057c', NULL, 0, 0, NULL, 0, 0)
INSERT INTO [dbo].[AspNetUsers] ([Id], [NombreCliente], [ApellidoCliente1], [ApellidoCliente2], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'7d6fc6db-1cef-41fd-9fe3-e61363eff3a9', N'Marta', N'Tosca', N'Perez', N'Marta', NULL, NULL, NULL, 0, NULL, N'8544c36f-2761-4030-944d-175dfb5e91ee', N'61d60f4a-8ab7-4208-9d7a-4f7c57417296', NULL, 0, 0, NULL, 0, 0)


SET IDENTITY_INSERT [dbo].[TiposBocadillos] ON
INSERT INTO [dbo].[TiposBocadillos] ([IdTipo], [NombreTipo]) VALUES (1, N'Normal')
INSERT INTO [dbo].[TiposBocadillos] ([IdTipo], [NombreTipo]) VALUES (2, N'Veganos')
INSERT INTO [dbo].[TiposBocadillos] ([IdTipo], [NombreTipo]) VALUES (3, N'Vegetarianos')
INSERT INTO [dbo].[TiposBocadillos] ([IdTipo], [NombreTipo]) VALUES (4, N'Sin gluten')
SET IDENTITY_INSERT [dbo].[TiposBocadillos] OFF

SET IDENTITY_INSERT [dbo].[BonosBocadillos] ON
INSERT INTO [dbo].[BonosBocadillos] ([BonoId], [CantidadDisponible], [NBocadillos], [Nombre], [PVP], [TipoBocadillosIdTipo]) VALUES (1, 5, 10, N'Completo', 10, 1)
INSERT INTO [dbo].[BonosBocadillos] ([BonoId], [CantidadDisponible], [NBocadillos], [Nombre], [PVP], [TipoBocadillosIdTipo]) VALUES (2, 3, 10, N'Mixto', 10, 2)
INSERT INTO [dbo].[BonosBocadillos] ([BonoId], [CantidadDisponible], [NBocadillos], [Nombre], [PVP], [TipoBocadillosIdTipo]) VALUES (3, 4, 15, N'Vegano', 15, 3)
SET IDENTITY_INSERT [dbo].[BonosBocadillos] OFF

SET IDENTITY_INSERT [dbo].[MetodoPago] ON
INSERT INTO [dbo].[MetodoPago] ([metodoPagoId], [metodoName], [Discriminator]) VALUES (1, N'Tarjeta', N'1')
INSERT INTO [dbo].[MetodoPago] ([metodoPagoId], [metodoName], [Discriminator]) VALUES (2, N'GooglePay', N'2')
INSERT INTO [dbo].[MetodoPago] ([metodoPagoId], [metodoName], [Discriminator]) VALUES (4, N'Paypal', N'3')
SET IDENTITY_INSERT [dbo].[MetodoPago] OFF

SET IDENTITY_INSERT [dbo].[ComprasBono] ON
INSERT INTO [dbo].[ComprasBono] ([CompraBonoId], [FechaCompraBono], [metodoPagoId], [NBonos], [PrecioTotalBono], [ClienteId]) VALUES (1, N'2025-11-05 00:00:00', 1, 2, 20, N'1')
SET IDENTITY_INSERT [dbo].[ComprasBono] OFF

SET IDENTITY_INSERT [dbo].[BonosComprados] ON
INSERT INTO [dbo].[BonosComprados] ([Id], [CompraId], [BonoId], [Cantidad], [PrecioBono]) VALUES (1, 1, 1, 2, 20)
SET IDENTITY_INSERT [dbo].[BonosComprados] OFF


--------MIGUEL----------------

SET IDENTITY_INSERT [dbo].[TipoProducto] ON
INSERT INTO [dbo].[TipoProducto] ([Productoid], [NombreProducto]) VALUES (1, N'Camiseta')
INSERT INTO [dbo].[TipoProducto] ([Productoid], [NombreProducto]) VALUES (2, N'Pantalon')
INSERT INTO [dbo].[TipoProducto] ([Productoid], [NombreProducto]) VALUES (3, N'Chaqueta')
SET IDENTITY_INSERT [dbo].[TipoProducto] OFF


SET IDENTITY_INSERT [dbo].[Producto] ON
INSERT INTO [dbo].[Producto] ([Productoid], [NombreProducto], [PVP], [Stock], [TipoProductoProductoid]) VALUES (1, N'Camiseta', 4, 5, 1)
INSERT INTO [dbo].[Producto] ([Productoid], [NombreProducto], [PVP], [Stock], [TipoProductoProductoid]) VALUES (2, N'Pantalon', 3, 10, 2)
INSERT INTO [dbo].[Producto] ([Productoid], [NombreProducto], [PVP], [Stock], [TipoProductoProductoid]) VALUES (3, N'Chaqueta', 50, 9, 3)
SET IDENTITY_INSERT [dbo].[Producto] OFF


SET IDENTITY_INSERT [dbo].[Compra_Producto] ON
INSERT INTO [dbo].[Compra_Producto] ([Id], [CompraId], [ClienteId], [DireccionEnvio], [FechaCompra], [Metodo_PagometodoPagoId], [PrecioFinal]) VALUES (5, 1, N'1', N'Calle Echegaray', N'2025-12-12 00:00:00', 1, 50)
SET IDENTITY_INSERT [dbo].[Compra_Producto] OFF


INSERT INTO [dbo].[Producto_Compra] ([Compraid], [Productoid], [Id], [PVP], [Cantidad]) VALUES (5, 1, 5, 50, 3)


------------MARTA----------------

SET IDENTITY_INSERT [dbo].[TiposPan] ON
INSERT INTO [dbo].[TiposPan] ([PanId], [Nombre]) VALUES (1, N'integral')
SET IDENTITY_INSERT [dbo].[TiposPan] OFF


SET IDENTITY_INSERT [dbo].[Bocadillos] ON
INSERT INTO [dbo].[Bocadillos] ([Id], [ComprasDelBocadillo], [Nombre], [Pvp], [Resenyabocadillo], [Stock], [Tamano], [tipopanPanId]) VALUES (4, 3, N'jamon', 3, N'muy bien', 50, 0, 1)
INSERT INTO [dbo].[Bocadillos] ([Id], [ComprasDelBocadillo], [Nombre], [Pvp], [Resenyabocadillo], [Stock], [Tamano], [tipopanPanId]) VALUES (5, 3, N'jamon', 6, N'muy bien', 50, 0, 1)
SET IDENTITY_INSERT [dbo].[Bocadillos] OFF

SET IDENTITY_INSERT [dbo].[Compras] ON
INSERT INTO [dbo].[Compras] ([CompraId], [FechaCompra], [nBoadillos], [metodoPagoId], [PrecioTotal], [ApplicationUserId]) VALUES (2, N'2025-11-05 00:00:00', 2, 1, 10, N'1')
SET IDENTITY_INSERT [dbo].[Compras] OFF

SET IDENTITY_INSERT [dbo].[ComprasBocadillos] ON
INSERT INTO [dbo].[ComprasBocadillos] ([Id], [BocadilloId], [CompraId], [Cantidad], [NombreBocadillo], [Precio]) VALUES (3, 4, 2, 2, N'jamon', 3)
SET IDENTITY_INSERT [dbo].[ComprasBocadillos] OFF


-------------JESÚS---------------------
SET IDENTITY_INSERT [dbo].[TiposPan] ON
INSERT INTO [dbo].[TiposPan] ([PanId], [Nombre]) VALUES (3, N'Integral')
INSERT INTO [dbo].[TiposPan] ([PanId], [Nombre]) VALUES (4, N'Blanco')
SET IDENTITY_INSERT [dbo].[TiposPan] OFF

SET IDENTITY_INSERT [dbo].[Bocadillos] ON
INSERT INTO [dbo].[Bocadillos] ([Id], [ComprasDelBocadillo], [Nombre], [Pvp], [Resenyabocadillo], [Stock], [Tamano], [tipopanPanId]) VALUES (1, 1, N'Submarino', 6, N'Bien', 5, 10, 4)
SET IDENTITY_INSERT [dbo].[Bocadillos] OFF

SET IDENTITY_INSERT [dbo].[Resenyas] ON
INSERT INTO [dbo].[Resenyas] ([Id], [Descripcion], [FechaPublicacion], [NombreUsuario], [Titulo], [Valoracion]) VALUES (1, N'Bien', N'2024-11-11 00:00:00', N'Miguel', N'R1', 7)
INSERT INTO [dbo].[Resenyas] ([Id], [Descripcion], [FechaPublicacion], [NombreUsuario], [Titulo], [Valoracion]) VALUES (2, N'Bien esta buenisimo ', N'2025-11-04 10:06:02', N'Miguel', N'R12', 0)
INSERT INTO [dbo].[Resenyas] ([Id], [Descripcion], [FechaPublicacion], [NombreUsuario], [Titulo], [Valoracion]) VALUES (3, N'esta malisimo lo odio', N'2025-11-05 08:42:58', N'marta', N'R13', 10)
INSERT INTO [dbo].[Resenyas] ([Id], [Descripcion], [FechaPublicacion], [NombreUsuario], [Titulo], [Valoracion]) VALUES (4, N'madremia que asco', N'2025-11-05 09:31:36', N'jesus', N'R14', 7)
INSERT INTO [dbo].[Resenyas] ([Id], [Descripcion], [FechaPublicacion], [NombreUsuario], [Titulo], [Valoracion]) VALUES (5, N'madremia que asco', N'2025-11-05 09:31:39', N'jesus', N'R14', 7)
SET IDENTITY_INSERT [dbo].[Resenyas] OFF

INSERT INTO [dbo].[ResenyasBocadillo] ([BocadilloId], [ResenyaId], [Id], [Puntuacion]) VALUES (1, 1, 1, 8)
INSERT INTO [dbo].[ResenyasBocadillo] ([BocadilloId], [ResenyaId], [Id], [Puntuacion]) VALUES (1, 2, 0, 5)
INSERT INTO [dbo].[ResenyasBocadillo] ([BocadilloId], [ResenyaId], [Id], [Puntuacion]) VALUES (1, 3, 0, 10)
INSERT INTO [dbo].[ResenyasBocadillo] ([BocadilloId], [ResenyaId], [Id], [Puntuacion]) VALUES (1, 4, 0, 10)
INSERT INTO [dbo].[ResenyasBocadillo] ([BocadilloId], [ResenyaId], [Id], [Puntuacion]) VALUES (1, 5, 0, 10)







