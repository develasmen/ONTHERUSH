using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONTHERUSH.AccesoADatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSolicitudCambioRuta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesCambioRuta",
                columns: table => new
                {
                    SolicitudCambioRutaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViajeId = table.Column<int>(type: "int", nullable: false),
                    ConductorId = table.Column<int>(type: "int", nullable: false),
                    RutaActual = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NuevoOrdenPropuesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentarioAdministrador = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCambioRuta", x => x.SolicitudCambioRutaId);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioRuta_Conductores_ConductorId",
                        column: x => x.ConductorId,
                        principalTable: "Conductores",
                        principalColumn: "ConductorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioRuta_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "ViajeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioRuta_ConductorId",
                table: "SolicitudesCambioRuta",
                column: "ConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioRuta_ViajeId",
                table: "SolicitudesCambioRuta",
                column: "ViajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesCambioRuta");
        }
    }
}
