namespace ONTHERUSH.UI.Helpers
{
    using ONTHERUSH.Abstracciones.DTOs;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public static class PdfHelper
    {
        public static byte[] GenerarReporteViajes(List<ReporteViajeDTO> datos)
        {
            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Reporte de Viajes")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Ruta").Bold();
                            h.Cell().Text("Salida").Bold();
                            h.Cell().Text("Conductor").Bold();
                            h.Cell().Text("Estado").Bold();
                        });

                        foreach (var v in datos)
                        {
                            table.Cell().Text(v.Ruta);
                            table.Cell().Text(v.Salida.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Text(v.Conductor);
                            table.Cell().Text(v.Estado);
                        }
                    });
                });
            }).GeneratePdf();
        }

        public static byte[] GenerarReporteUsuarios(List<UsuarioReporteDTO> datos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // HEADER
                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Usuarios")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        col.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10)
                            .AlignCenter();
                    });

                    // CONTENT
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        // Columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Nombre
                            columns.RelativeColumn(3); // Email
                            columns.RelativeColumn(2); // Fecha
                            columns.RelativeColumn(2); // Tipo
                        });

                        // HEADER TABLE
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Nombre").Bold();
                            header.Cell().Element(CellStyle).Text("Email").Bold();
                            header.Cell().Element(CellStyle).Text("Fecha Registro").Bold();
                            header.Cell().Element(CellStyle).Text("Tipo Usuario").Bold();

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Padding(5)
                                    .Background(Colors.Grey.Lighten2)
                                    .Border(1);
                            }
                        });

                        // DATA
                        foreach (var item in datos)
                        {
                            table.Cell().Element(CellStyle).Text(item.NombreCompleto);
                            table.Cell().Element(CellStyle).Text(item.Email);
                            table.Cell().Element(CellStyle).Text(item.FechaRegistro.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).Text(item.TipoUsuario);

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Padding(5)
                                    .Border(1);
                            }
                        }
                    });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerarReporteVehiculos(List<VehiculoReporteDTO> datos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Vehículos")
                            .FontSize(20).Bold().AlignCenter();

                        col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).AlignCenter();
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Placa
                            columns.RelativeColumn(3); // Marca
                            columns.RelativeColumn(1); // Año
                            columns.RelativeColumn(2); // Capacidad
                            columns.RelativeColumn(2); // Estado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Placa").Bold();
                            header.Cell().Element(CellStyle).Text("Marca/Modelo").Bold();
                            header.Cell().Element(CellStyle).Text("Año").Bold();
                            header.Cell().Element(CellStyle).Text("Capacidad").Bold();
                            header.Cell().Element(CellStyle).Text("Estado").Bold();

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Padding(5)
                                    .Background(Colors.Grey.Lighten2)
                                    .Border(1);
                            }

                        });

                        foreach (var v in datos)
                        {
                            table.Cell().Element(Cell).Text(v.Placa);
                            table.Cell().Element(Cell).Text(v.MarcaModelo);
                            table.Cell().Element(Cell).Text(v.Annio.ToString());
                            table.Cell().Element(Cell).Text(v.Capacidad.ToString());
                            table.Cell().Element(Cell).Text(v.Estado);

                            static IContainer Cell(IContainer c) =>
                                c.Padding(5).Border(1);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerarReporteReservas(List<ReservaReporteDTO> datos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Reservas")
                            .FontSize(20).Bold().AlignCenter();

                        col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).AlignCenter();
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID").Bold();
                            header.Cell().Element(CellStyle).Text("Cliente").Bold();
                            header.Cell().Element(CellStyle).Text("Ruta").Bold();
                            header.Cell().Element(CellStyle).Text("Estado").Bold();
                            header.Cell().Element(CellStyle).Text("Fecha de Reserva").Bold();

                            static IContainer CellStyle(IContainer c) =>
                                c.Padding(5).Border(1);
                        });

                        foreach (var r in datos)
                        {
                            table.Cell().Element(Cell).Text(r.ReservaId.ToString());
                            table.Cell().Element(Cell).Text(r.Cliente);
                            table.Cell().Element(Cell).Text(r.Ruta);
                            table.Cell().Element(Cell).Text(r.FechaReserva.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Cell).Text(r.Estado);

                            static IContainer Cell(IContainer c) =>
                                c.Padding(5).Border(1);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}