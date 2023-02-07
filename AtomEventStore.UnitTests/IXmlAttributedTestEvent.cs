namespace Grean.AtomEventStore.UnitTests
{
    public interface IXmlAttributedTestEvent
    {
        IXmlAttributedTestEventVisitor Accept(
            IXmlAttributedTestEventVisitor visitor);
    }
}
