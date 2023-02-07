using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests
{
    [DataContract(Name = "test-event-y", Namespace = "http://grean.rocks/dc")]
    public class DataContractTestEventY : IDataContractTestEvent
    {
        [DataMember(Name = "number")]
        public long Number { get; set; }

        [DataMember(Name = "text")]
        public bool IsTrue { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as DataContractTestEventY;
            if (other != null)
                return object.Equals(this.Number, other.Number)
                    && object.Equals(this.IsTrue, other.IsTrue);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return
                this.Number.GetHashCode() ^
                this.IsTrue.GetHashCode();
        }

        public IDataContractTestEventVisitor Accept(
            IDataContractTestEventVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
