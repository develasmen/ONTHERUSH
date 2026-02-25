using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONTHERUSH.AccesoADatos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarReservaYAgregarSolicitudCambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Viajes_ViajeId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaCancelacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ParadaDestino",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "ViajeId",
                table: "Reservas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Reservas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Canton",
                table: "Reservas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Distrito",
                table: "Reservas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraDestino",
                table: "Reservas",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "Reservas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SolicitudesCambio",
                columns: table => new
                {
                    SolicitudCambioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoCambio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValorActual = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValorNuevo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCambio", x => x.SolicitudCambioId);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambio_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambio_UsuarioId",
                table: "SolicitudesCambio",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Viajes_ViajeId",
                table: "Reservas",
                column: "ViajeId",
                principalTable: "Viajes",
                principalColumn: "ViajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Viajes_ViajeId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "SolicitudesCambio");

            migrationBuilder.DropColumn(
                name: "Canton",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Distrito",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HoraDestino",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "ViajeId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Reservas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCancelacion",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParadaDestino",
                table: "Reservas",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Viajes_ViajeId",
                table: "Reservas",
                column: "ViajeId",
                principalTable: "Viajes",
                principalColumn: "ViajeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
