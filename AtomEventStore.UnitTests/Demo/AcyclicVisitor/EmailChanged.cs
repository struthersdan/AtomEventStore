using System;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests.Demo.AcyclicVisitor
{
    [DataContract(Name = "email-changed", Namespace = "urn:grean:samples:user-on-boarding")]
    public class EmailChanged
    {
        [DataMember(Name = "user-id")]
        public Guid UserId { get; set; }

        [DataMember(Name = "new-email")]
        public string NewEmail { get; set; }
    }
}
