using System;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests.Demo.Visitor
{
    [DataContract(Name = "user-created", Namespace = "urn:grean:samples:user-sign-up")]
    public class UserCreated : IUserEvent
    {
        [DataMember(Name = "user-id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "user-name")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        public IUserVisitor Accept(IUserVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
