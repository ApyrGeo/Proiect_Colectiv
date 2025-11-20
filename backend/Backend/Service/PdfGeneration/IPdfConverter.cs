namespace TrackForUBB.Service.PdfGeneration;

public interface IPdfConverter
{
    Task Convert(string documentPath, string outputDirectory);
}
