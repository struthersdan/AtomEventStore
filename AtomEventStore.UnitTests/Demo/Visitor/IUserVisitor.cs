namespace Grean.AtomEventStore.UnitTests.Demo.Visitor
{
    public interface IUserVisitor
    {
        IUserVisitor Visit(UserCreated @event);

        IUserVisitor Visit(EmailVerified @event);

        IUserVisitor Visit(EmailChanged @event);
    }
}
