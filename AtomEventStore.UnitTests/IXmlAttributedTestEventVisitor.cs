namespace Grean.AtomEventStore.UnitTests
{
    public interface IXmlAttributedTestEventVisitor
    {
        IXmlAttributedTestEventVisitor Visit(XmlAttributedTestEventX tex);

        IXmlAttributedTestEventVisitor Visit(XmlAttributedTestEventY tey);
    }
}
