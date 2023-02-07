namespace Grean.AtomEventStore.UnitTests
{
    public interface IDataContractTestEventVisitor
    {
        IDataContractTestEventVisitor Visit(DataContractTestEventX tex);

        IDataContractTestEventVisitor Visit(DataContractTestEventY tey);
    }
}
