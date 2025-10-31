using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII2526.API.Migrations
{
    /// <inheritdoc />
    public partial class sprint2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Compra_Compra_Producto_Compraid",
                table: "Producto_Compra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Producto_Compra",
                table: "Producto_Compra");

            migrationBuilder.DropIndex(
                name: "IX_Producto_Compra_Compraid",
                table: "Producto_Compra");

            migrationBuilder.DropColumn(
                name: "Apellido_1",
                table: "Compra_Producto");

            migrationBuilder.DropColumn(
                name: "Apellido_2",
                table: "Compra_Producto");

            migrationBuilder.DropColumn(
                name: "Metodo_Pago",
                table: "Compra_Producto");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Compra_Producto");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "TipoProducto",
                newName: "NombreProducto");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Producto_Compra",
                newName: "Compraid1");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Producto",
                newName: "NombreProducto");

            migrationBuilder.RenameColumn(
                name: "Surname1",
                table: "AspNetUsers",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Sruename2",
                table: "AspNetUsers",
                newName: "Apellido2");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "Apellido1");

            migrationBuilder.AlterColumn<int>(
                name: "Compraid",
                table: "Producto_Compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Compraid1",
                table: "Producto_Compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<double>(
                name: "PVP",
                table: "Producto",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "Compra_Producto",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Metodo_PagometodoPagoId",
                table: "Compra_Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Producto_Compra",
                table: "Producto_Compra",
                column: "Compraid");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Compra_Compraid1",
                table: "Producto_Compra",
                column: "Compraid1");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_Producto_ClienteId",
                table: "Compra_Producto",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_Producto_Metodo_PagometodoPagoId",
                table: "Compra_Producto",
                column: "Metodo_PagometodoPagoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Producto_AspNetUsers_ClienteId",
                table: "Compra_Producto",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compra_Producto_MetodoPago_Metodo_PagometodoPagoId",
                table: "Compra_Producto",
                column: "Metodo_PagometodoPagoId",
                principalTable: "MetodoPago",
                principalColumn: "metodoPagoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Compra_Compra_Producto_Compraid1",
                table: "Producto_Compra",
                column: "Compraid1",
                principalTable: "Compra_Producto",
                principalColumn: "Compraid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Producto_AspNetUsers_ClienteId",
                table: "Compra_Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Compra_Producto_MetodoPago_Metodo_PagometodoPagoId",
                table: "Compra_Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Compra_Compra_Producto_Compraid1",
                table: "Producto_Compra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Producto_Compra",
                table: "Producto_Compra");

            migrationBuilder.DropIndex(
                name: "IX_Producto_Compra_Compraid1",
                table: "Producto_Compra");

            migrationBuilder.DropIndex(
                name: "IX_Compra_Producto_ClienteId",
                table: "Compra_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Compra_Producto_Metodo_PagometodoPagoId",
                table: "Compra_Producto");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Compra_Producto");

            migrationBuilder.DropColumn(
                name: "Metodo_PagometodoPagoId",
                table: "Compra_Producto");

            migrationBuilder.RenameColumn(
                name: "NombreProducto",
                table: "TipoProducto",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Compraid1",
                table: "Producto_Compra",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NombreProducto",
                table: "Producto",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "AspNetUsers",
                newName: "Surname1");

            migrationBuilder.RenameColumn(
                name: "Apellido2",
                table: "AspNetUsers",
                newName: "Sruename2");

            migrationBuilder.RenameColumn(
                name: "Apellido1",
                table: "AspNetUsers",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "Compraid",
                table: "Producto_Compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Producto_Compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "PVP",
                table: "Producto",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Apellido_1",
                table: "Compra_Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Apellido_2",
                table: "Compra_Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Metodo_Pago",
                table: "Compra_Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Compra_Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Producto_Compra",
                table: "Producto_Compra",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Compra_Compraid",
                table: "Producto_Compra",
                column: "Compraid");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Compra_Compra_Producto_Compraid",
                table: "Producto_Compra",
                column: "Compraid",
                principalTable: "Compra_Producto",
                principalColumn: "Compraid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
