using System.Xml.Linq;

namespace TrackForUBB.Service.PdfGeneration;

public interface IXmlTemplateFiller
{
    void Fill(XDocument document, object model);
}
