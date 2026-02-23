using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONTHERUSH.AccesoADatos.Migrations
{
    /// <inheritdoc />
    public partial class CorregirColumnaSolicitudCambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesCambio_AspNetUsers_UsuarioId",
                table: "SolicitudesCambio");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SolicitudesCambio");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "SolicitudesCambio",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesCambio_AspNetUsers_UsuarioId",
                table: "SolicitudesCambio",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesCambio_AspNetUsers_UsuarioId",
                table: "SolicitudesCambio");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "SolicitudesCambio",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SolicitudesCambio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesCambio_AspNetUsers_UsuarioId",
                table: "SolicitudesCambio",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
