namespace Grean.AtomEventStore.UnitTests
{
    public class TypeResolutionEntryTests
    {
        [Theory, AutoAtomData]
        public void VerifyConstructorInitializedProperties(
            ConstructorInitializedMemberAssertion assertion)
        {
            assertion.Verify(typeof(TypeResolutionEntry).GetProperties());
        }

        [Theory, AutoAtomData]
        public void VerifyGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(TypeResolutionEntry));
        }
    }
}
