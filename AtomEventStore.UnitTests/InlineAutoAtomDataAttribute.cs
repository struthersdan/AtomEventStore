namespace Grean.AtomEventStore.UnitTests
{
    public class InlineAutoAtomDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoAtomDataAttribute(params object[] values)
            : base(new AutoAtomDataAttribute(), values)
        {
        }
    }
}
