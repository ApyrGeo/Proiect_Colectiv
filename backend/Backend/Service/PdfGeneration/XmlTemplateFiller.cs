using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using log4net;

namespace TrackForUBB.Service.PdfGeneration;

public class XmlTemplateFiller : IXmlTemplateFiller
{
    public XmlFillResult Fill(XDocument document, object model)
        => Fill(document.Root!, model);

    public XmlFillResult Fill(XElement root, object model)
    {
        globalResult = new([]);
        FillImpl(root, model);
        logger.DebugFormat("Image Counts: {0}", globalResult.ImagePathsToBase64.Count);
        FinishAddingImages(root);
        logger.DebugFormat("Image Counts 2: {0}", globalResult.ImagePathsToBase64.Count);
        return globalResult;
    }

    private void FinishAddingImages(XElement root)
    {
        if (globalResult.ImagePathsToBase64.Count == 0)
            return;

        var automaticStylesName = OfficeNS.GetName("automatic-styles");
        var automaticStylesElement = root.Elements(automaticStylesName).FirstOrDefault();
        if (automaticStylesElement is null)
        {
            automaticStylesElement = new XElement(automaticStylesName);
            root.AddFirst(automaticStylesElement);
        }

        var styleName = StyleNS.GetName("style");
        var styleElement = new XElement(styleName);
        {
            var styleElementAttributes = new List<(string Name, string Value)>
            {
                ("name", imageStyleKey),
                ("family", "graphic"),
                ("parent-style-name", "Graphics"),
            };
            foreach (var entry in styleElementAttributes)
                styleElement.SetAttributeValue(StyleNS.GetName(entry.Name), entry.Value);

            var graphicElement = new XElement(StyleNS.GetName("graphic-properties"));
            var graphicElementAttributes = new List<(XNamespace Ns, string Name, string Value)>
            {
                (StyleNS, "run-through", "foreground"),
                (StyleNS, "wrap", "run-through"),
                (StyleNS, "number-wrapped-paragraphs", "no-limit"),
                (StyleNS, "vertical-pos", "middle"),
                (StyleNS, "vertical-rel", "char"),
                (StyleNS, "horizontal-pos", "left"),
                (StyleNS, "horizontal-rel", "char"),
                (StyleNS, "mirror", "none"),
                (FoNS, "clip", "rect(0cm, 0cm, 0cm, 0cm)"),
                (DrawNS, "luminance", "0%"),
                (DrawNS, "contrast", "0%"),
                (DrawNS, "red", "0%"),
                (DrawNS, "green", "0%"),
                (DrawNS, "blue", "0%"),
                (DrawNS, "gamma", "100%"),
                (DrawNS, "color-inversion", "true"),
                (DrawNS, "image-opacity", "100%"),
                (DrawNS, "color-mode", "standard"),
            };
            foreach (var entry in graphicElementAttributes)
                graphicElement.SetAttributeValue(entry.Ns.GetName(entry.Name), entry.Value);
            styleElement.Add(graphicElement);
        }
        automaticStylesElement.Add(styleElement);
    }

    public void FillImpl(XElement root, object model) {
        if (getElementText(root) is string) {
            ReplacePlaceholders(root, model);
            return;
        }

        foreach (var child in root.Elements().ToList()) {
            if (tryGetLoop(child, out var loopName, out var loopBody, out var addBeforeThis))
            {
                var iterable = GetIterableProperty(model, loopName);

                foreach (var value in iterable)
                {
                    foreach (var element in loopBody)
                    {
                        var copy = new XElement(element);
                        FillImpl(copy, value);
                        addBeforeThis.AddBeforeSelf(copy);
                    }
                }

                addBeforeThis.Remove();
            } else {
                FillImpl(child, model);
            }
        }
    }

    static readonly Regex placeholderRegex = new Regex("{{((?<type>[a-z0-9_/]+):)?(?<placeholder>[a-z_][a-z0-9_]*?)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private IEnumerable<XNode> ReplacePlaceholders(string content, object model) {
        var m = placeholderRegex.Match(content);
        if (!m.Success)
        {
            yield return new XText(content);
            yield break;
        }

        var lastIndex = 0;
        foreach (Match match in placeholderRegex.Matches(content))
        {
            if (match.Index != lastIndex)
                yield return new XText(content.Substring(lastIndex, match.Index - lastIndex));

            lastIndex = match.Index + match.Length;
            var type = match.Groups["type"].Value;
            var propertyName = match.Groups["placeholder"].Value;
            var property = GetProperty(model, propertyName);
            var propertyValue = property?.ToString();

            if (string.IsNullOrEmpty(propertyValue))
                continue;

            switch (type)
            {
                case "":
                    yield return new XText(propertyValue);
                    break;
                case "image":
                    yield return ComputeImageNode(propertyName, propertyValue);
                    break;
                default:
                    logger.ErrorFormat("Unknown placeholder type: '{type}'", type);
                    yield return new XText($$$"""{{ERROR: unknowon type {{{type}}}}}""");
                    break;
            }
        }

        if (lastIndex < content.Length)
            yield return new XText(content.Substring(lastIndex));
    }

    private XNode ComputeImageNode(string name, string base64Value)
    {
        var result = new XElement(DrawNS.GetName("frame"));
        result.SetAttributeValue(DrawNS.GetName("style-name"), imageStyleKey);
        result.SetAttributeValue(DrawNS.GetName("name"), name);
        result.SetAttributeValue(TextNS + "anchor-type", "char");
        result.SetAttributeValue(SvgNS + "x", "0cm");
        result.SetAttributeValue(SvgNS + "width", "2cm");
        result.SetAttributeValue(SvgNS + "height", "1cm");
        result.SetAttributeValue(DrawNS + "z-index", "0");

        var imageElement = new XElement(DrawNS + "image");
        result.Add(imageElement);
        var imagePathInArchive = $"Pictures/FillerImage{name}.png";
        imageElement.SetAttributeValue(XLinkNS + "href", imagePathInArchive);
        imageElement.SetAttributeValue(XLinkNS + "type", "simple");
        imageElement.SetAttributeValue(XLinkNS + "show", "embed");
        imageElement.SetAttributeValue(XLinkNS + "actuate", "onLoad");
        imageElement.SetAttributeValue(DrawNS + "mime-type", "image/png");

        globalResult.ImagePathsToBase64.TryAdd(imagePathInArchive, base64Value);

        return result;
    }

    private void ReplacePlaceholders(XNode node, object model)
    {
        if (node is XText nodeT)
            node.ReplaceWith(ReplacePlaceholders(nodeT.Value, model));
        else if (node is XElement nodeE)
        {
            foreach (var child in nodeE.Nodes().ToList())
                ReplacePlaceholders(child, model);
        } else
            throw new Exception("ReplacePlaceholders received a node of type: " + node.NodeType);
    }

    private object? GetProperty(object model, string name)
    {
        if (name == "this")
            return model;

        var prop = model.GetType().GetProperty(name);
        FieldInfo? field = null;
        if (prop is null)
            field = model.GetType().GetField(name);

        if (prop is null && field is null)
            throw new Exception($"Could not find property {name} in context {model}");

        return prop?.GetValue(model) ?? field?.GetValue(model);
    }

    private bool tryGetLoop(
        XElement element,
        [NotNullWhen(true)] out string? loopName,
        [NotNullWhen(true)] out List<XElement> loopBody,
        [NotNullWhen(true)] out XElement? addBeforeThis
    )
    {
        loopName = null;
        loopBody = [];
        addBeforeThis = null;

        var text = getElementText(element);
        if (text == null || !text.StartsWith("{{#") || !text.EndsWith("}}"))
            return false;

        loopName = text[3 .. ^2];
        var endMarker = "{{/" + loopName + "}}";

        foreach (var next in element.ElementsAfterSelf().ToList()) {
            next.PreviousNode?.Remove();

            if (getElementText(next) == endMarker) {
                addBeforeThis = next;
                break;
            }

            loopBody.Add(next);
        }
        if (addBeforeThis is null)
            throw new Exception($"Didn't find end of {loopName} loop");

        var listTag = TextNS.GetName("list");
        if (loopBody.HasSingle(out var listElement) && listElement.Name == listTag) {
            loopBody = listElement.Elements().ToList();

            var marker = new XElement("marker");
            listElement.ReplaceNodes(marker);
            addBeforeThis.AddBeforeSelf(listElement);
            addBeforeThis.Remove();
            addBeforeThis = marker;
        }

        var tableTag = TableNS + "table";
        if (loopBody.HasSingle(out var tableElement) && tableElement.Name == tableTag) {
            var lastElement = tableElement.Elements().LastOrDefault();

            if (lastElement?.Name != TableNS + "table-row")
                return true;

            var marker = new XElement("marker");
            lastElement.AddBeforeSelf(marker);
            lastElement.Remove();

            loopBody = [ lastElement ];

            addBeforeThis.AddBeforeSelf(tableElement);
            addBeforeThis.Remove();
            addBeforeThis = marker;
        }

        return true;
    }

    private string? getElementText(XElement element)
    {
        if (element.NodeType == XmlNodeType.Text)
            return element.Value;

        var allText = element
            .Nodes()
            .All(x => x.NodeType == XmlNodeType.Text || x is XElement el && el.Name.LocalName == "span");

        return allText ? element.Value : null;
    }

    private IEnumerable<object> GetIterableProperty(object model, string name)
    {
        return GetProperty(model, name) switch {
            null => [],
            IEnumerable<object> @enum => @enum,
            IEnumerable @untypedEnum => @untypedEnum.Cast<object>(),
            _ => throw new Exception($"Property {name} in context {model} is not iterable"),
        };
    }

    private XmlFillResult globalResult = new([]);

    private static readonly XNamespace OfficeNS = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
    private static readonly XNamespace StyleNS = "urn:oasis:names:tc:opendocument:xmlns:style:1.0" ;
    private static readonly XNamespace TextNS = "urn:oasis:names:tc:opendocument:xmlns:text:1.0" ;
    private static readonly XNamespace TableNS = "urn:oasis:names:tc:opendocument:xmlns:table:1.0" ;
    private static readonly XNamespace DrawNS = "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0" ;
    private static readonly XNamespace FoNS = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0" ;
    private static readonly XNamespace SvgNS = "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0" ;
    private static readonly XNamespace XLinkNS = "http://www.w3.org/1999/xlink" ;
    private static readonly string imageStyleKey = "_fillter-default-image-style";

    private ILog logger = LogManager.GetLogger(typeof(XmlTemplateFiller));
}

file static class EnumExtensions {
    public static bool HasSingle<T>(this IEnumerable<T> @enum, [NotNullWhen(true)] out T? result) {
        result = default(T);

        int i = 0;
        foreach (var element in @enum) {
            result = element;
            ++ i;
            if (i > 1) break;
        }

        return i == 1;
    }
}
