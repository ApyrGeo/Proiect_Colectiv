using System.Diagnostics;

namespace TrackForUBB.Service.PdfGeneration;

public class PdfConverter(PdfConverterConfiguration config) : IPdfConverter
{
    public async Task Convert(string docPath, string outputDirectory)
    {
        var soffice = config.SOfficePath;
        var p = new Process
        {
            StartInfo = new ProcessStartInfo(soffice, [
                "--headless",
                "--convert-to", "pdf:writer_pdf_Export",
                "--outdir", outputDirectory,
                docPath,
            ])
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        p.Start();
        await p.WaitForExitAsync();

        if (p.ExitCode != 0)
            throw new Exception("Converting to pdf: soffice exited with status: " + p.ExitCode);

        var expectedFilename = Path.Combine(outputDirectory, Path.GetFileName(docPath));
        if (!Path.Exists(expectedFilename))
            throw new Exception("Converting to pdf: soffice didn't produce the expected file");
    }
}
