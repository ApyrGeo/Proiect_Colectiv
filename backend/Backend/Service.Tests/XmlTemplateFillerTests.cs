using System.Xml.Linq;
using TrackForUBB.Service.PdfGeneration;

namespace TrackForUBB.Service.Tests;

public class XmlTemplateFillerTests
{
    private readonly IXmlTemplateFiller filler;
    public XmlTemplateFillerTests()
    {
        filler = new XmlTemplateFiller();
    }


    [Fact]
    public void SubstituteText()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName, "Hello, my name is {{this}} and I like test driver development")
            ));
        filler.Fill(root, "Maria");
        var expected = new XDocument(
            new XElement("root",
                new XElement(PName, "Hello, my name is Maria and I like test driver development"))
        );
        Assert.Equal(expected, root);
    }

    [Fact]
    public void SubstitueTextInNestedSpan()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Hello, my name is ",
                    new XElement(SpanName, "{{this}}"),
                    " and I like test driven development"
                )
            )
        );
        filler.Fill(root, "Maria");
        var expected = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Hello, my name is ",
                    new XElement(SpanName, "Maria"),
                    " and I like test driven development"
                )
            )
        );
        Assert.Equal(expected, root);
    }

    [Fact(Skip = "Unimplemented")]
    public void SubstituteTextInNestedSpanFullyInside()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "What's up, ",
                    new XElement(SpanName,
                        "{",
                        new XElement(SpanName,
                            "{",
                            new XElement(SpanName,
                                "thi"),
                            "s"),
                        "}}")
                )
            )
        );
        filler.Fill(root, "Doc");
        var expected = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "What's up, ",
                    new XElement(SpanName, "Maria")
                )
            )
        );
        Assert.Equal(expected, root);
    }

    [Fact(Skip = "Unimplemented")]
    public void SubstituteTextInNestedSpanPartiallyOutside()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "What's up, {{", new XElement(SpanName, "this"), "}}?"
                )
            )
        );
        filler.Fill(root, "Doc");
        var expected = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "What's up, Doc?"
                )
            )
        );
        Assert.Equal(expected, root);
    }

    [Fact]
    public void SubstitueList()
    {
        var document = new XDocument(
            new XElement("root",
                new XElement(PName, "{{#List}}"),
                new XElement(ListName,
                    new XElement(ListItemName, "{{A}}"),
                    new XElement(ListItemName,
                        new XElement(PName, "With love from {{X}} and {{Y}}")
                    )
                ),
                new XElement(PName, "{{/List}}"),
                new XElement(PName, "after list")
            )
        );
        var list = new[]{
            new { A = "A1", X = "X1", Y = "Y1" },
            new { A = "A2", X = "X2", Y = "Y2" },
        };

        filler.Fill(document, new { List = list });

        var expected = new XDocument(
            new XElement("root",
                new XElement(ListName,
                    new XElement(ListItemName, "A1"),
                    new XElement(ListItemName,
                        new XElement(PName, "With love from X1 and Y1")
                    ),
                    new XElement(ListItemName, "A2"),
                    new XElement(ListItemName,
                        new XElement(PName, "With love from X2 and Y2")
                    )
                ),
                new XElement(PName, "after list")
            )
        );

        Assert.Equal(expected, document);
    }

    [Fact]
    public void SubstituteTableLoop()
    {
        var document = new XDocument(
            new XElement("root",
                new XElement(PName, "Before table!"),
                new XElement(PName, "{{#this}}"),

                new XElement(TableName,
                    new XElement(TableColumnName),

                    new XElement(TableRowName,
                        new XElement(TableCellName, "Header 1"),
                        new XElement(TableCellName, "Header 1"),
                        new XElement(TableCellName, "Header 2")
                    ),

                    new XElement(TableRowName,
                        new XElement(TableCellName, "Merged column 1")
                    ),

                    new XElement(TableRowName,
                        new XElement(TableCellName,
                            "{{A}}"
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Hello {{B}}"
                            )
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Bye ",
                                new XElement(SpanName,
                                    "{{C}}"
                                )
                            )
                        )
                    )

                ),
                new XElement(PName, "{{/this}}"),
                new XElement(PName, "after table")
            )
        );

        var list = new[]{
            new { A = "A1", B = "B1", C = "C1" },
            new { A = "A2", B = "B2", C = "C2" },
        };

        filler.Fill(document, list);

        var expected = new XDocument(
            new XElement("root",
                new XElement(PName, "Before table!"),

                new XElement(TableName,
                    new XElement(TableColumnName),

                    new XElement(TableRowName,
                        new XElement(TableCellName, "Header 1"),
                        new XElement(TableCellName, "Header 1"),
                        new XElement(TableCellName, "Header 2")
                    ),

                    new XElement(TableRowName,
                        new XElement(TableCellName, "Merged column 1")
                    ),

                    new XElement(TableRowName,
                        new XElement(TableCellName,
                            "A1"
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Hello B1"
                            )
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Bye ",
                                new XElement(SpanName,
                                    "C1"
                                )
                            )
                        )
                    ),

                    new XElement(TableRowName,
                        new XElement(TableCellName,
                            "A2"
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Hello B2"
                            )
                        ),
                        new XElement(TableCellName,
                            new XElement(PName,
                                "Bye ",
                                new XElement(SpanName,
                                    "C2"
                                )
                            )
                        )
                    )

                ),

                new XElement(PName, "after table")
            )
        );

        Assert.Equal(expected, document);
    }

    [Fact]
    public void SubstituteText_NoMatch()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in here"
                )
            )
        );
        filler.Fill(root, "text");
        var expected = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in here"
                )
            )
        );
        Assert.Equal(expected, root);
        var p = root.Root!.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_MatchBeggining()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "{{this}}Match in here"
                )
            )
        );
        filler.Fill(root, "text");
        Assert.Equal("textMatch in here", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_OneMatchMiddle()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in {{this}} here"
                )
            )
        );
        filler.Fill(root, "text");
        Assert.Equal("Match in text here", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_OneMatchEnd()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in here {{this}}"
                )
            )
        );
        filler.Fill(root, "text");
        Assert.Equal("Match in here text", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_AdjancentMiddle()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in {{a}}{{b}} here"
                )
            )
        );
        filler.Fill(root, new { a = "a", b = "b" });
        Assert.Equal("Match in ab here", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_AdjancentBeginning()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "{{a}}{{b}} Match in here"
                )
            )
        );
        filler.Fill(root, new { a = "a", b = "b" });
        Assert.Equal("ab Match in here", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_AdjancentEnd()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Match in here {{a}}{{b}}"
                )
            )
        );
        filler.Fill(root, new { a = "a", b = "b" });
        Assert.Equal("Match in here ab", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_AllProperties()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "{{a}}{{b}}{{a}}"
                )
            )
        );
        filler.Fill(root, new { a = "a", b = "b" });
        Assert.Equal("aba", root.Root!.Value);
        var p = root.Root.Elements().Single();
        Assert.DoesNotContain(p.Nodes(), x => x is XText text && text.Value == string.Empty);
    }

    [Fact]
    public void SubstituteText_BadPlaceholderType()
    {
        var root = new XDocument(
            new XElement("root",
                new XElement(PName,
                    "Text {{mustar:this}} after"
                )
            )
        );
        filler.Fill(root, "pattern");
        var p = root.Root!.Elements().Single();
        var text = p.Value;
        Assert.Contains("Text", text);
        Assert.Contains("after", text);
        Assert.Contains("mustar", text);
        Assert.DoesNotContain("pattern", text);
        Assert.DoesNotContain("{{mustar:this}}", text);
    }

    private static XName PName => textNS.GetName("p");
    private static XName SpanName => textNS.GetName("span");

    private static XName ListName => textNS.GetName("list");
    private static XName ListItemName => textNS.GetName("list-item");

    private static XName TableName => tableNS.GetName("table");
    private static XName TableColumnName => tableNS.GetName("table-column");
    private static XName TableRowName => tableNS.GetName("table-row");
    private static XName TableCellName => tableNS.GetName("table-cell");

    private static readonly XNamespace textNS = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
    private static readonly XNamespace tableNS = "urn:oasis:names:tc:opendocument:xmlns:table:1.0";
}
