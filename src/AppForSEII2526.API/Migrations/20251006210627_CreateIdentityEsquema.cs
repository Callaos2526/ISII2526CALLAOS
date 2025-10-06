using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII2526.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateIdentityEsquema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sruename2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compra_Producto",
                columns: table => new
                {
                    Compraid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo_Pago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioFinal = table.Column<int>(type: "int", nullable: false),
                    Apellido_1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido_2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compra_Producto", x => x.Compraid);
                });

            migrationBuilder.CreateTable(
                name: "MetodoPago",
                columns: table => new
                {
                    metodoPagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metodoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodoPago", x => x.metodoPagoId);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Productoid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PVP = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Productoid);
                });

            migrationBuilder.CreateTable(
                name: "Resenyas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resenyas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposBocadillos",
                columns: table => new
                {
                    IdTipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposBocadillos", x => x.IdTipo);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    CompraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido_1Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido_2Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nBoadillos = table.Column<int>(type: "int", nullable: false),
                    metodoPagoId = table.Column<int>(type: "int", nullable: false),
                    PrecioTotal = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.CompraId);
                    table.ForeignKey(
                        name: "FK_Compras_MetodoPago_metodoPagoId",
                        column: x => x.metodoPagoId,
                        principalTable: "MetodoPago",
                        principalColumn: "metodoPagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComprasBono",
                columns: table => new
                {
                    CompraBonoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApellidoBono1 = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ApellidoBono2 = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FechaCompraBono = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoPagoId = table.Column<int>(type: "int", nullable: false),
                    NBonos = table.Column<int>(type: "int", nullable: false),
                    PrecioTotalBono = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasBono", x => x.CompraBonoId);
                    table.ForeignKey(
                        name: "FK_ComprasBono_MetodoPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "MetodoPago",
                        principalColumn: "metodoPagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto_Compra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Compraid = table.Column<int>(type: "int", nullable: false),
                    Productoid = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PVP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto_Compra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_Compra_Compra_Producto_Compraid",
                        column: x => x.Compraid,
                        principalTable: "Compra_Producto",
                        principalColumn: "Compraid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Compra_Producto_Productoid",
                        column: x => x.Productoid,
                        principalTable: "Producto",
                        principalColumn: "Productoid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoProducto",
                columns: table => new
                {
                    Productoid = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProducto", x => x.Productoid);
                    table.ForeignKey(
                        name: "FK_TipoProducto_Producto_Productoid",
                        column: x => x.Productoid,
                        principalTable: "Producto",
                        principalColumn: "Productoid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonosBocadillos",
                columns: table => new
                {
                    BonoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false),
                    NBocadillos = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PVP = table.Column<double>(type: "float", nullable: false),
                    TipoBocadillosIdTipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonosBocadillos", x => x.BonoId);
                    table.ForeignKey(
                        name: "FK_BonosBocadillos_TiposBocadillos_TipoBocadillosIdTipo",
                        column: x => x.TipoBocadillosIdTipo,
                        principalTable: "TiposBocadillos",
                        principalColumn: "IdTipo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonosComprados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    BonoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioBono = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonosComprados", x => x.Id);
                    table.UniqueConstraint("AK_BonosComprados_CompraId_BonoId", x => new { x.CompraId, x.BonoId });
                    table.ForeignKey(
                        name: "FK_BonosComprados_BonosBocadillos_BonoId",
                        column: x => x.BonoId,
                        principalTable: "BonosBocadillos",
                        principalColumn: "BonoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonosComprados_ComprasBono_CompraId",
                        column: x => x.CompraId,
                        principalTable: "ComprasBono",
                        principalColumn: "CompraBonoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bocadillos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComprasDelBocadillo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pvp = table.Column<float>(type: "real", nullable: false),
                    Resenyabocadillo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Tamano = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tamaño = table.Column<int>(type: "int", nullable: false),
                    tipopanPanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bocadillos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComprasBocadillos",
                columns: table => new
                {
                    BocadilloId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    NombreBocadillo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasBocadillos", x => new { x.CompraId, x.BocadilloId });
                    table.ForeignKey(
                        name: "FK_ComprasBocadillos_Bocadillos_BocadilloId",
                        column: x => x.BocadilloId,
                        principalTable: "Bocadillos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComprasBocadillos_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "CompraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResenyasBocadillo",
                columns: table => new
                {
                    BocadilloId = table.Column<int>(type: "int", nullable: false),
                    ResenyaId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Puntuacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResenyasBocadillo", x => new { x.BocadilloId, x.ResenyaId });
                    table.ForeignKey(
                        name: "FK_ResenyasBocadillo_Bocadillos_BocadilloId",
                        column: x => x.BocadilloId,
                        principalTable: "Bocadillos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResenyasBocadillo_Resenyas_ResenyaId",
                        column: x => x.ResenyaId,
                        principalTable: "Resenyas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposPan",
                columns: table => new
                {
                    PanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompraBocadilloBocadilloId = table.Column<int>(type: "int", nullable: true),
                    CompraBocadilloCompraId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPan", x => x.PanId);
                    table.ForeignKey(
                        name: "FK_TiposPan_ComprasBocadillos_CompraBocadilloCompraId_CompraBocadilloBocadilloId",
                        columns: x => new { x.CompraBocadilloCompraId, x.CompraBocadilloBocadilloId },
                        principalTable: "ComprasBocadillos",
                        principalColumns: new[] { "CompraId", "BocadilloId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bocadillos_tipopanPanId",
                table: "Bocadillos",
                column: "tipopanPanId");

            migrationBuilder.CreateIndex(
                name: "IX_BonosBocadillos_TipoBocadillosIdTipo",
                table: "BonosBocadillos",
                column: "TipoBocadillosIdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_BonosComprados_BonoId",
                table: "BonosComprados",
                column: "BonoId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_metodoPagoId",
                table: "Compras",
                column: "metodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasBocadillos_BocadilloId",
                table: "ComprasBocadillos",
                column: "BocadilloId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasBono_MetodoPagoId",
                table: "ComprasBono",
                column: "MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Compra_Compraid",
                table: "Producto_Compra",
                column: "Compraid");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Compra_Productoid",
                table: "Producto_Compra",
                column: "Productoid");

            migrationBuilder.CreateIndex(
                name: "IX_ResenyasBocadillo_ResenyaId",
                table: "ResenyasBocadillo",
                column: "ResenyaId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPan_CompraBocadilloCompraId_CompraBocadilloBocadilloId",
                table: "TiposPan",
                columns: new[] { "CompraBocadilloCompraId", "CompraBocadilloBocadilloId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Bocadillos_TiposPan_tipopanPanId",
                table: "Bocadillos",
                column: "tipopanPanId",
                principalTable: "TiposPan",
                principalColumn: "PanId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bocadillos_TiposPan_tipopanPanId",
                table: "Bocadillos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BonosComprados");

            migrationBuilder.DropTable(
                name: "Producto_Compra");

            migrationBuilder.DropTable(
                name: "ResenyasBocadillo");

            migrationBuilder.DropTable(
                name: "TipoProducto");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BonosBocadillos");

            migrationBuilder.DropTable(
                name: "ComprasBono");

            migrationBuilder.DropTable(
                name: "Compra_Producto");

            migrationBuilder.DropTable(
                name: "Resenyas");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "TiposBocadillos");

            migrationBuilder.DropTable(
                name: "TiposPan");

            migrationBuilder.DropTable(
                name: "ComprasBocadillos");

            migrationBuilder.DropTable(
                name: "Bocadillos");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "MetodoPago");
        }
    }
}
