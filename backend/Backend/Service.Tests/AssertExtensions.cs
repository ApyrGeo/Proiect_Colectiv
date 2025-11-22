using System.Xml.Linq;

namespace Xunit;

public partial class Assert
{
    public static void Equal(XDocument expected, XDocument actual)
    {
        var areEqual = XDocument.DeepEquals(expected, actual);
        if (!areEqual)
            Assert.Fail($"Expected:\n{expected}\nActual:\n{actual}");
    }
}
