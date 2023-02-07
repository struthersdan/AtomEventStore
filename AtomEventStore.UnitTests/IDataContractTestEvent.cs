namespace Grean.AtomEventStore.UnitTests
{
    public interface IDataContractTestEvent
    {
        IDataContractTestEventVisitor Accept(IDataContractTestEventVisitor visitor);
    }
}
