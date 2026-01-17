using System.IO.Compression;
using System.Xml.Linq;

namespace TrackForUBB.Service.PdfGeneration;

public class DocumentTemplateFiller(IXmlTemplateFiller xmlFiller) : IDocumentTemplateFiller
{
    public async Task GenerateFile(string inputPath, object model, string outputFile, CancellationToken ct = default)
    {
        using var inMemory = new MemoryStream();
        using (var fileStream = new FileStream(inputPath, FileMode.Open))
        {
            await fileStream.CopyToAsync(inMemory, ct);
        }
        using (var archive = new ZipArchive(inMemory, ZipArchiveMode.Update, true))
        {
            var contentEntry = archive.GetEntry("content.xml");
            if (contentEntry is null)
            {
                throw new Exception($"Archive {inputPath} doesn't appear to be a odt (no content.xml)");
            }

            XmlFillResult fillResult;
            XDocument contentDocument;
            using (var contentStream = contentEntry.Open())
            {
                contentDocument = await XDocument.LoadAsync(contentStream, LoadOptions.PreserveWhitespace, ct);
                fillResult = xmlFiller.Fill(contentDocument, model);
            }

            using (var contentStream = contentEntry.Open())
            {
                using var contentWriteStream = new StreamWriter(contentStream);
                contentDocument.Save(contentWriteStream);
            }

            await AddImages(fillResult.ImagePathsToBase64, archive);
        }
        ct.ThrowIfCancellationRequested();

        using var outputStream = File.Create(outputFile);
        inMemory.Position = 0;
        await inMemory.CopyToAsync(outputStream, ct);
    }

    private async Task AddImages(Dictionary<string, string> imagePathsToBase64, ZipArchive archive, CancellationToken ct = default)
    {
        var metaEntry = archive.GetEntry("META-INF/manifest.xml")
            ?? throw new Exception("No META-INF/manifest.xml inside of document");

        XDocument metaDocument;
        using (var metaStream = metaEntry.Open())
        {
            metaDocument = await XDocument.LoadAsync(metaStream, LoadOptions.PreserveWhitespace, ct);
        }

        ct.ThrowIfCancellationRequested();
        var manifestRoot = metaDocument.Element(ManifestNS + "manifest")
            ?? throw new Exception("Document has no manifest root");
        
        foreach (var path in imagePathsToBase64.Keys)
        {
            var element = new XElement(ManifestNS + "file-entry");
            element.SetAttributeValue(ManifestNS + "full-path", path);
            element.SetAttributeValue(ManifestNS + "media-type", "image/png");
            manifestRoot.Add(element);
        }
        
        using (var metaStream = metaEntry.Open())
        {
            using var writeStream = new StreamWriter(metaStream);
            metaDocument.Save(writeStream);
        }

        foreach (var entry in imagePathsToBase64)
        {
            var file = archive.CreateEntry(entry.Key);
            using var stream = file.Open();
            var fileData = Convert.FromBase64String(entry.Value);
            await stream.WriteAsync(fileData);
        }
    }

    private static readonly XNamespace ManifestNS = "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";
}
