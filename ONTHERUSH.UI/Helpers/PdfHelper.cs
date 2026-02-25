namespace ONTHERUSH.UI.Helpers
{
    using ONTHERUSH.Abstracciones.DTOs;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using System.Reflection.Metadata;

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
    }
}
