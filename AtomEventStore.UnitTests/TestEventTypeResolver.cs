using System;

namespace Grean.AtomEventStore.UnitTests
{
    public class TestEventTypeResolver : ITypeResolver
    {
        public Type Resolve(string localName, string xmlNamespace)
        {
            switch (xmlNamespace)
            {
                case "http://grean:rocks":
                    switch (localName)
                    {
                        case "test-event-x":
                            return typeof(XmlAttributedTestEventX);
                        case "test-event-y":
                            return typeof(XmlAttributedTestEventY);
                        default:
                            throw new ArgumentException("Unexpected local name.", "localName");
                    }
                default:
                    throw new ArgumentException("Unexpected XML namespace.", "xmlNamespace");
            }
        }
    }
}
