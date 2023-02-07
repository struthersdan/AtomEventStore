﻿using System.IO;
using System.Text;
using System.Xml;

namespace Grean.AtomEventStore.UnitTests
{
    internal static class XmlAttributedTestEventEnvy
    {
        internal static string AsSerializedString(
            this IXmlAttributedTestEvent @event,
            IContentSerializer serializer)
        {
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb))
            {
                serializer.Serialize(w, @event);
                w.Flush();
            }
            return sb.ToString();
        }

        internal static object RoundTrip(
            this IXmlAttributedTestEvent @event,
            IContentSerializer serializer)
        {
            using (var ms = new MemoryStream())
            using (var w = XmlWriter.Create(ms))
            {
                serializer.Serialize(w, @event);
                w.Flush();
                ms.Position = 0;
                using (var r = XmlReader.Create(ms))
                    return serializer.Deserialize(r).Item;
            }
        }
    }
}
