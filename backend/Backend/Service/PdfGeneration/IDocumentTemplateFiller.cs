namespace TrackForUBB.Service.PdfGeneration;

public interface IDocumentTemplateFiller
{
    public Task GenerateFile(string documentPath, object model, string outputDirectory, CancellationToken ct = default);
}
