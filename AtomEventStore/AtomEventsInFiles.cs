﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Grean.AtomEventStore
{
    /// <summary>
    /// Stores events in Atom Feeds as files on disk.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Suppressed following discussion at http://bit.ly/11T4eZe")]
    public class AtomEventsInFiles : IAtomEventStorage, IEnumerable<UuidIri>
    {
        private readonly DirectoryInfo directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomEventsInFiles"/>
        /// class.
        /// </summary>
        /// <param name="directory">
        /// The base directory that will be used to store the Atom files.
        /// </param>
        /// <remarks>
        /// <para>
        /// The <paramref name="directory" /> argument is subsequently
        /// available via the <see cref="Directory" /> property.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="directory" /> is <see langword="null" />.
        /// </exception>
        public AtomEventsInFiles(DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            this.directory = directory;
        }

        /// <summary>
        /// Creates an <see cref="XmlReader" /> for reading an Atom feed from
        /// the provided <see cref="Uri" />.
        /// </summary>
        /// <param name="href">
        /// The relative <see cref="Uri" /> of the Atom feed to read.
        /// </param>
        /// <returns>
        /// An <see cref="XmlReader" /> over the Atom feed identified by
        /// <paramref name="href" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="href" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method attempts to find a file corresponding to
        /// <paramref name="href" />. If the file is found, an
        /// <see cref="XmlReader" /> over that file is created and returned. If
        /// the file isn't found, an XmlReader over an empty Atom Feed is
        /// returned. In this case, no file is created for the empty Atom Feed.
        /// In other words: this method has no observable side-effects.
        /// </para>
        /// </remarks>
        public XmlReader CreateFeedReaderFor(Uri href)
        {
            if (href == null)
                throw new ArgumentNullException("href");

            var fileName = this.CreateFileName(href);

            if (File.Exists(fileName))
                return XmlReader.Create(fileName);

            return AtomEventStorage.CreateNewFeed(href);
        }

        /// <summary>
        /// Creates an <see cref="XmlWriter" /> for writing the provided
        /// <see cref="AtomFeed" />.
        /// </summary>
        /// <param name="atomFeed">The Atom feed to write.</param>
        /// <returns>
        /// An <see cref="XmlWriter" /> over the Atom feed provided by
        /// <paramref name="atomFeed" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="atomFeed" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method returns an <see cref="XmlWriter" /> for a particular
        /// Atom Feed. The file name and location is based on the Atom Feed's
        /// "self" link.
        /// </para>
        /// </remarks>
        public XmlWriter CreateFeedWriterFor(AtomFeed atomFeed)
        {
            if (atomFeed == null)
                throw new ArgumentNullException("atomFeed");

            var fileName = this.CreateFileName(atomFeed.Links);
            var dir = Path.GetDirectoryName(fileName);
            System.IO.Directory.CreateDirectory(dir);
            return XmlWriter.Create(fileName);
        }

        /// <summary>
        /// Gets the based directory.
        /// </summary>
        /// <value>
        /// The base directory supplied via the constructor.
        /// </value>
        /// <seealso cref="AtomEventsInFiles" />
        public DirectoryInfo Directory
        {
            get { return this.directory; }
        }

        private string CreateFileName(IEnumerable<AtomLink> links)
        {
            var selfLink = links.Single(l => l.IsSelfLink);
            return this.CreateFileName(selfLink.Href);
        }

        private string CreateFileName(Uri href)
        {
            return Path.Combine(
                this.directory.ToString(),
                href.ToString() + ".xml");
        }

        /// <summary>
        /// Returns an enumerator that iterates through all the event stream
        /// IDs in <see cref="Directory" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that
        /// can be used to iterate through the IDs.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The base <see cref="Directory" /> can contain multiple event
        /// streams, each identified by an event stream ID. This Iterator
        /// enumerates all the event stream IDs. A client can use this to find
        /// all the IDs in the collection represented by this AtomEventsInFiles
        /// instance.
        /// </para>
        /// </remarks>
        /// <seealso cref="AtomEventObserver{T}" />
        /// <seealso cref="FifoEvents{T}" />
        public IEnumerator<UuidIri> GetEnumerator()
        {
            return this.directory
                .EnumerateDirectories()
                .Select(d => TryParseGuid(d.Name))
                .Where(t => t.Item1)
                .Select(t => (UuidIri)t.Item2)
                .GetEnumerator();
        }

        private static Tuple<bool, Guid> TryParseGuid(string candidate)
        {
            Guid id;
            var success = Guid.TryParse(candidate, out id);
            return new Tuple<bool, Guid>(success, id);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can
        /// be used to iterate through the collection.
        /// </returns>
        /// <seealso cref="GetEnumerator()" />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
