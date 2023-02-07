﻿using System;

namespace Grean.AtomEventStore
{
    /// <summary>
    /// Represents an Entry in a <see cref="TypeResolutionTable"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The TypeResolutionEntry class represents a set of required data in
    /// order to construct a valid <see cref="TypeResolutionTable"/>.
    /// </para>
    /// </remarks>
    public class TypeResolutionEntry
    {
        private readonly string xmlNamespace;
        private readonly string localName;
        private readonly Type resolvedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolutionEntry"/>
        /// class.
        /// </summary>
        /// <param name="xmlNamespace">The XML namespace of the XML name.
        /// </param>
        /// <param name="localName">The local name of the XML name.</param>
        /// <param name="resolvedType">The <see cref="Type"/> resolved by the
        /// XML namespace and the local name of the XML name.
        /// </param>
        /// <remarks>
        /// <para>
        /// The constructor arguments are subsequently available on the object
        /// as properties.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="xmlNamespace" />
        /// or
        /// <paramref name="localName" />
        /// or
        /// <paramref name="resolvedType" /> is <see langword="null" />.
        /// </exception>
        public TypeResolutionEntry(
            string xmlNamespace,
            string localName,
            Type resolvedType)
        {
            if (xmlNamespace == null)
                throw new ArgumentNullException("xmlNamespace");
            if (localName == null)
                throw new ArgumentNullException("localName");
            if (resolvedType == null)
                throw new ArgumentNullException("resolvedType");

            this.xmlNamespace = xmlNamespace;
            this.localName = localName;
            this.resolvedType = resolvedType;
        }

        /// <summary>
        /// Gets the XML namespace of the <see cref="TypeResolutionEntry"/>.
        /// </summary>
        /// <value>
        /// The XML namespace of the <see cref="TypeResolutionEntry"/>, as
        /// originally provided via the constructor.
        /// </value>
        /// <seealso cref="TypeResolutionEntry(string, string, Type)" />
        public string XmlNamespace
        {
            get { return this.xmlNamespace; }
        }

        /// <summary>
        /// Gets the local name of the <see cref="TypeResolutionEntry"/>.
        /// </summary>
        /// <value>
        /// The local name of the <see cref="TypeResolutionEntry"/>, as
        /// originally provided via the constructor.
        /// </value>
        /// <seealso cref="TypeResolutionEntry(string, string, Type)" />
        public string LocalName
        {
            get { return this.localName; }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> resolved by the XML namespace and the
        /// local name of the <see cref="TypeResolutionEntry"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> resolved by the XML namespace and the local
        /// name of the <see cref="TypeResolutionEntry"/>, as originally
        /// provided via the constructor.
        /// </value>
        /// <seealso cref="TypeResolutionEntry(string, string, Type)" />
        public Type ResolvedType
        {
            get { return this.resolvedType; }
        }
    }
}
