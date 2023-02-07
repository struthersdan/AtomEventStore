﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Grean.AtomEventStore.UnitTests
{
    public class TypeResolutionTableTests
    {
        [Fact]
        public void SutIsTypeResolver()
        {
            var sut = new TypeResolutionTable();
            Assert.IsAssignableFrom<ITypeResolver>(sut);
        }

        [Theory, AutoAtomData]
        public void VerifyGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(TypeResolutionTable));
        }

        [Theory, AutoAtomData]
        public void EntriesIsCorrectWhenInitializedWithArray(
            [Frozen]TypeResolutionEntry[] expected,
            [FavorArrays]TypeResolutionTable sut)
        {
            var actual = sut.Entries;
            Assert.True(expected.SequenceEqual(actual));
        }

        [Theory, AutoAtomData]
        public void EntriesIsCorrectWhenInitializedWithEnumerable(
            [Frozen]IReadOnlyCollection<TypeResolutionEntry> expected,
            [FavorEnumerables]TypeResolutionTable sut)
        {
            var actual = sut.Entries;
            Assert.True(expected.SequenceEqual(actual));
        }

        [Theory, AutoAtomData]
        public void ResolveReturnsCorrectResult(
            TypeResolutionTable sut,
            Fixture fixture)
        {
            var entry = sut.Entries.ToArray().PickRandom();
            var expected = entry.ResolvedType;

            var actual = sut.Resolve(entry.LocalName, entry.XmlNamespace);

            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void ResolveThrowsWhenInputCanNotBeMappedToProperType(
            TypeResolutionTable sut,
            TypeResolutionEntry notMapped)
        {
            Assert.False(sut.Entries.Any(x =>
                x.LocalName == notMapped.LocalName &&
                x.XmlNamespace == notMapped.XmlNamespace));

            Assert.Throws<ArgumentException>(() =>
                sut.Resolve(notMapped.LocalName, notMapped.XmlNamespace));
        }

        [Theory, AutoAtomData]
        public void ResolveThrowsForUnmappedLocalNameAndMappedXmlNamespace(
            TypeResolutionTable sut,
            TypeResolutionEntry notMapped)
        {
            Assert.False(sut.Entries.Any(x =>
                x.LocalName == notMapped.LocalName &&
                x.XmlNamespace == notMapped.XmlNamespace));
            var mapped = sut.Entries.ToArray().PickRandom();

            Assert.Throws<ArgumentException>(() =>
                sut.Resolve(notMapped.LocalName, mapped.XmlNamespace));
        }

        [Theory, AutoAtomData]
        public void ResolveThrowsForMappedLocalNameAndUnmappedXmlNamespace(
            TypeResolutionTable sut,
            TypeResolutionEntry notMapped)
        {
            Assert.False(sut.Entries.Any(x =>
                x.LocalName == notMapped.LocalName &&
                x.XmlNamespace == notMapped.XmlNamespace));
            var mapped = sut.Entries.ToArray().PickRandom();

            Assert.Throws<ArgumentException>(() =>
                sut.Resolve(mapped.LocalName, notMapped.XmlNamespace));
        }
    }
}
