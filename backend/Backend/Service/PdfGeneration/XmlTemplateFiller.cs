using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace TrackForUBB.Service.PdfGeneration;

public class XmlTemplateFiller : IXmlTemplateFiller
{
    public void Fill(XDocument document, object model)
        => Fill(document.Root!, model);

    public void Fill(XElement root, object model) {
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
                        Fill(copy, value);
                        addBeforeThis.AddBeforeSelf(copy);
                    }
                }

                addBeforeThis.Remove();
            } else {
                Fill(child, model);
            }
        }
    }

    static readonly Regex placeholderRegex = new Regex("{{(?<placeholder>[a-z_][a-z0-9_]*?)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private string ReplacePlaceholders(string content, object model) {
        return placeholderRegex.Replace(content, m =>
        {
            var name = m.Groups["placeholder"].Value;
            var value = GetProperty(model, name);
            return value?.ToString() ?? string.Empty;
        });
    }

    private void ReplacePlaceholders(XNode node, object model) {
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

        var listTag = XName.Get("list", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
        if (loopBody.HasSingle(out var listElement) && listElement.Name == listTag) {
            loopBody = listElement.Elements().ToList();

            var marker = new XElement("marker");
            listElement.ReplaceNodes(marker);
            addBeforeThis.AddBeforeSelf(listElement);
            addBeforeThis.Remove();
            addBeforeThis = marker;
        }

        var tableNamespace = XNamespace.Get("urn:oasis:names:tc:opendocument:xmlns:table:1.0");
        var tableTag = tableNamespace + "table";
        if (loopBody.HasSingle(out var tableElement) && tableElement.Name == tableTag) {
            var lastElement = tableElement.Elements().LastOrDefault();

            if (lastElement?.Name != tableNamespace + "table-row")
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
