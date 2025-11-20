namespace TrackForUBB.Service.PdfGeneration;

public class PdfGenerator(IDocumentTemplateFiller documentFiller, IPdfConverter converter) : IPdfGenerator
{
    public async Task<byte[]> Generate(string documentPath, object model)
    {
        var tempDirectory = Directory.CreateTempSubdirectory("trackforubb_pdf_");
        try
        {
            var docOutputPath = Path.Combine(tempDirectory.FullName, "doc.odt");
            await documentFiller.GenerateFile(documentPath, model, docOutputPath);

            await converter.Convert(docOutputPath, tempDirectory.FullName);

            var pdfOutputPath = Path.ChangeExtension(docOutputPath, "pdf");
            return await File.ReadAllBytesAsync(pdfOutputPath);
        }
        finally
        {
            tempDirectory.Delete(true);
        }
    }
}
