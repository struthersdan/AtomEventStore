namespace Grean.AtomEventStore.UnitTests.Demo.Visitor
{
    public interface IUserEvent
    {
        IUserVisitor Accept(IUserVisitor visitor);
    }
}
