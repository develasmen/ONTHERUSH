using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONTHERUSH.AccesoADatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaUltimaModificacionViaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaModificacion",
                table: "Viajes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaUltimaModificacion",
                table: "Viajes");
        }
    }
}
