using System;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests.Demo.AcyclicVisitor
{
    [DataContract(Name = "user-created", Namespace = "urn:grean:samples:user-on-boarding")]
    public class UserCreated
    {
        [DataMember(Name = "user-id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "user-name")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}
