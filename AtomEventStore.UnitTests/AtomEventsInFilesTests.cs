using System.IO;

namespace Grean.AtomEventStore.UnitTests
{
    public class AtomEventsInFilesTests
    {
        [Theory, AutoAtomData]
        public void SutIsAtomEventStorage(AtomEventsInFiles sut)
        {
            Assert.IsAssignableFrom<IAtomEventStorage>(sut);
        }

        [Theory, AutoAtomData]
        public void DirectoryIsCorrect(
            [Frozen]DirectoryInfo expected,
            AtomEventsInFiles sut)
        {
            DirectoryInfo actual = sut.Directory;
            Assert.Equal(expected, actual);
        }
    }
}
