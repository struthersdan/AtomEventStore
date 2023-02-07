using System;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests.Demo.Visitor
{
    [DataContract(Name = "email-changed", Namespace = "urn:grean:samples:user-sign-up")]
    public class EmailChanged : IUserEvent
    {
        [DataMember(Name = "user-id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "new-email")]
        public string NewEmail { get; set; }

        public IUserVisitor Accept(IUserVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
