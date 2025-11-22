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

            XDocument contentDocument;
            using (var contentStream = contentEntry.Open())
            {
                contentDocument = await XDocument.LoadAsync(contentStream, LoadOptions.PreserveWhitespace, ct);
                xmlFiller.Fill(contentDocument, model);
            }
            using (var contentWriteStream = new StreamWriter(contentEntry.Open()))
            {
                contentDocument.Save(contentWriteStream);
            }
        }

        ct.ThrowIfCancellationRequested();

        using var outputStream = File.Create(outputFile);
        inMemory.Position = 0;
        await inMemory.CopyToAsync(outputStream, ct);
    }
}
