namespace TrackForUBB.Service.PdfGeneration;

public interface IPdfGenerator
{
    Task<byte[]> Generate(string documentPath, object model);
}
