namespace TrackForUBB.Service.PdfGeneration;

public record XmlFillResult(
    Dictionary<string, string> ImagePathsToBase64
);
