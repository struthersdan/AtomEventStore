using System;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests.Demo.AcyclicVisitor
{
    [DataContract(Name = "email-verified", Namespace = "urn:grean:samples:user-on-boarding")]
    public class EmailVerified
    {
        [DataMember(Name = "user-id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}
