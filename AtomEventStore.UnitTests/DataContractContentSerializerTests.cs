﻿using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization;

namespace Grean.AtomEventStore.UnitTests
{
    public class DataContractContentSerializerTests
    {
        [Theory, AutoAtomData]
        public void SutIsContentSerializer(DataContractContentSerializer sut)
        {
            Assert.IsAssignableFrom<IContentSerializer>(sut);
        }

        [Theory, AutoAtomData]
        public void SerializeCorrectlySerializesAttributedClassInstance(
            DataContractContentSerializer sut,
            DataContractTestEventX dctex)
        {
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb))
            {
                sut.Serialize(w, dctex);
                w.Flush();
                var actual = sb.ToString();

                var expected = XDocument.Parse(
                    "<test-event-x xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://grean.rocks/dc\">" +
                    "  <number>" + dctex.Number + "</number>" +
                    "  <text>" + dctex.Text + "</text>" +
                    "</test-event-x>");
                Assert.Equal(expected, XDocument.Parse(actual), new XNodeEqualityComparer());
            }
        }

        [Theory, AutoAtomData]
        public void SutCanRoundTripAttributedClassInstance(
            DataContractContentSerializer sut,
            DataContractTestEventX dctex)
        {
            using (var ms = new MemoryStream())
            using (var w = XmlWriter.Create(ms))
            {
                sut.Serialize(w, dctex);
                w.Flush();
                ms.Position = 0;
                using (var r = XmlReader.Create(ms))
                {
                    var content = sut.Deserialize(r);

                    var actual = Assert.IsAssignableFrom<DataContractTestEventX>(content.Item);
                    Assert.Equal(dctex.Number, actual.Number);
                    Assert.Equal(dctex.Text, actual.Text);
                }
            }
        }

        [Fact]
        public void CreateTypeResolverWithNullAssemblyThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DataContractContentSerializer.CreateTypeResolver(null));
        }

        [Fact]
        public void CreateTypeResolverWithAssemblyWithoutAnnotatedTypesThrows()
        {
            var assembly = typeof(Version).Assembly;
            Assert.Empty(
                from t in assembly.GetExportedTypes()
                from a in t.GetCustomAttributes(
                              typeof(DataContractAttribute), inherit: false)
                           .Cast<DataContractAttribute>()
                where t.IsDefined(a.GetType(), inherit: false)
                select t);

            Assert.Throws<ArgumentException>(() =>
                DataContractContentSerializer.CreateTypeResolver(assembly));
        }

        [Fact]
        public void CreateTypeResolverReturnsCorrectResult()
        {
            var assemblyToScanForEvents =
                typeof(DataContractContentSerializerTests).Assembly;
            var mappings =
                (from t in assemblyToScanForEvents.GetExportedTypes()
                 from a in t.GetCustomAttributes(
                               typeof(DataContractAttribute), inherit: false)
                            .Cast<DataContractAttribute>()
                 where t.IsDefined(a.GetType(), inherit: false)
                 select new TypeResolutionEntry(a.Namespace, a.Name, t))
                 .ToArray();
            Assert.NotEmpty(mappings);
            var sut =
                DataContractContentSerializer.CreateTypeResolver(
                    assemblyToScanForEvents);
            Array.ForEach(mappings, entry =>
            {
                var expected = entry.ResolvedType;
                var actual = sut.Resolve(entry.LocalName, entry.XmlNamespace);
                Assert.Equal(expected, actual);
            });
        }

        [Fact]
        public void ScanWithNullAssemblyThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DataContractContentSerializer.Scan(null));
        }

        [Theory, AutoData]
        public void ScanCorrectlySerializesAttributedClassInstance(
            DataContractTestEventX @event)
        {
            var actual =
                DataContractContentSerializer.Scan(@event.GetType().Assembly);

           var expected = XDocument.Parse(
                "<test-event-x xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://grean.rocks/dc\">" +
                "  <number>" + @event.Number + "</number>" +
                "  <text>" + @event.Text + "</text>" +
                "</test-event-x>");
            Assert.Equal(
                expected,
                XDocument.Parse(@event.AsSerializedString(actual)),
                new XNodeEqualityComparer());
        }

        [Theory, AutoData]
        public void ScanCanRoundTripAttributedClassInstance(
            DataContractTestEventX @event)
        {
            var actual =
                DataContractContentSerializer.Scan(@event.GetType().Assembly);

            var expected =
                Assert.IsAssignableFrom<DataContractTestEventX>(
                    @event.RoundTrip(actual));
            Assert.Equal(expected.Number, @event.Number);
            Assert.Equal(expected.Text, @event.Text);
        }
    }
}
