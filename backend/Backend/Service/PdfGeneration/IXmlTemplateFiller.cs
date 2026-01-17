using System.Xml.Linq;

namespace TrackForUBB.Service.PdfGeneration;

public interface IXmlTemplateFiller
{
    XmlFillResult Fill(XDocument document, object model);
}
