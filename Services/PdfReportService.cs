namespace APIisBEESinItaly.Services
{
    using APIisBEESinItaly.Models;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Element;
    using System.Collections.Generic;
    using System.IO;

    public class PdfReportService
    {
        public byte[] GeneratePdf(List<Pet> pets)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Crear el PDF
                using (PdfWriter writer = new PdfWriter(ms))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        // Título del Reporte
                        document.Add(new Paragraph("Pets registered")
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetFontSize(18));

                        // Crear la tabla con 4 columnas
                        Table table = new Table(4);
                        table.AddHeaderCell("ID");
                        table.AddHeaderCell("Nombre");
                        table.AddHeaderCell("Raza");
                        table.AddHeaderCell("Edad");

                        // Agregar las filas de datos a la tabla
                        foreach (var pet in pets)
                        {
                            table.AddCell(pet.Id.ToString());
                            table.AddCell(pet.Name);
                            table.AddCell("tbc"/*pet.Breed*/);
                            table.AddCell("tbc"/*pet.Breed*/); //table.AddCell(pet.Age.ToString());
                        }

                        // Agregar la tabla al documento PDF
                        document.Add(table);
                    }
                }

                return ms.ToArray();
            }
        }
    }

}
