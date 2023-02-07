﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Grean.AtomEventStore.UnitTests
{
    public class AtomEventStreamTests
    {
#pragma warning disable 618
        [Theory, AutoAtomData]
        public void VerifyGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(
                typeof(AtomEventStream<DataContractTestEventX>).GetMembers()
                    .Where(m => new Methods<AtomEventStream<DataContractTestEventX>>().Select(x => x.OnError(null)) != m));
        }

        [Theory, AutoAtomData]
        public void IdIsCorrect(
            [Frozen]UuidIri expected,
            AtomEventStream<DataContractTestEventX> sut)
        {
            UuidIri actual = sut.Id;
            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void StorageIsCorrect(
            [Frozen]IAtomEventStorage expected,
            AtomEventStream<DataContractTestEventX> sut)
        {
            IAtomEventStorage actual = sut.Storage;
            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void ContentSerializerIsCorrect(
            [Frozen]IContentSerializer expected,
            AtomEventStream<DataContractTestEventX> sut)
        {
            IContentSerializer actual = sut.ContentSerializer;
            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void CreateSelfLinkFromReturnsCorrectResult(
            UuidIri id)
        {
            AtomLink actual = AtomEventStream.CreateSelfLinkFrom(id);

            var expected = AtomLink.CreateSelfLink(
                new Uri(
                    ((Guid)id).ToString(),
                    UriKind.Relative));
            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void CreatePreviousLinkFromReturnsCorrectResult(
            UuidIri id)
        {
            AtomLink actual = AtomEventStream.CreatePreviousLinkFrom(id);

            var expected = AtomLink.CreatePreviousLink(
                new Uri(
                    ((Guid)id).ToString(),
                    UriKind.Relative));
            Assert.Equal(expected, actual);
        }

        [Theory, AutoAtomData]
        public void IsPreviousFeedLinkReturnsTrueForPreviousFeedLink(
            UuidIri id)
        {
            var link = AtomEventStream.CreatePreviousLinkFrom(id);
            bool actual = AtomEventStream.IsPreviousFeedLink(link);
            Assert.True(actual);
        }

        [Theory, AutoAtomData]
        public void IsPreviousFeedLinkReturnsFalsForLinkWithIncorrectRel(
            UuidIri id)
        {
            var link = AtomEventStream.CreateSelfLinkFrom(id);
            var actual = AtomEventStream.IsPreviousFeedLink(link);
            Assert.False(actual);
        }

        [Theory, AutoAtomData]
        public void IsPreviousFeedLinkReturnsFalseForLinkWithAbsoluteUri(
            Uri uri)
        {
            Assert.True(uri.IsAbsoluteUri);
            var link = AtomLink.CreatePreviousLink(uri);

            var actual = AtomEventStream.IsPreviousFeedLink(link);

            Assert.False(actual);
        }

        [Theory, AutoAtomData]
        public void IsPreviousFeedLinkReturnsFalseForLinkNotUuid(
            int number)
        {
            var uri = new Uri(number.ToString(), UriKind.Relative);
            var link = AtomLink.CreatePreviousLink(uri);

            var actual = AtomEventStream.IsPreviousFeedLink(link);

            Assert.False(actual);
        }

        [Theory, AutoAtomData]
        public void AppendAsyncCorrectlyStoresFeed(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            XmlAttributedTestEventX expectedEvent)
        {
            var before = DateTimeOffset.Now;

            sut.AppendAsync(expectedEvent).Wait();

            var writtenFeed = storage.Feeds.Select(parser.Parse).Single();
            var expectedFeed = new AtomFeedLikeness(before, sut.Id, expectedEvent);
            Assert.True(
                expectedFeed.Equals(writtenFeed),
                "Expected feed must match actual feed.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncCorrectlyStoresFeeds(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            XmlAttributedTestEventX event1,
            XmlAttributedTestEventX event2)
        {
            var before = DateTimeOffset.Now;

            sut.AppendAsync(event1).Wait();
            sut.AppendAsync(event2).Wait();

            var writtenFeed = storage.Feeds.Select(parser.Parse).Single();
            var expectedFeed =
                new AtomFeedLikeness(before, sut.Id, event2, event1);
            Assert.True(
                expectedFeed.Equals(writtenFeed),
                "Expected feed must match actual feed.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncPageSizeEventsSavesAllEntriesInIndex(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenFeed = storage.Feeds.Select(parser.Parse).Single();
            var expectedFeed = new AtomFeedLikeness(
                before,
                sut.Id,
                events.AsEnumerable().Reverse().ToArray());
            Assert.True(
                expectedFeed.Equals(writtenFeed),
                "Expected feed must match actual feed.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanPageSizeEventsOnlyStoresOverflowingEvent(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            var expectedIndex = new AtomFeedLikeness(
                before,
                sut.Id,
                events.AsEnumerable().Reverse().First());
            Assert.True(
                expectedIndex.Equals(writtenIndex),
                "Expected feed must match actual feed.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanPageSizeEventsAddsLinkToPreviousPageToIndex(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            Assert.Equal(
                1,
                writtenIndex.Links.Count(AtomEventStream.IsPreviousFeedLink));
        }

        [Theory, AutoAtomData]
        public void AppendAsyncLessThanPageSizeEventsDoesNotAddLinkToPreviousPageToIndex(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            Assert.Equal(
                0,
                writtenIndex.Links.Count(AtomEventStream.IsPreviousFeedLink));
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanPageSizeEventsStoresPreviousPage(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            UuidIri previousPageId = 
                Guid.Parse(
                    writtenIndex.Links
                        .Single(AtomEventStream.IsPreviousFeedLink)
                        .Href.ToString());
            Assert.True(
                storage.Feeds
                    .Select(parser.Parse)
                    .Any(f => f.Id == previousPageId),
                "The previous feed should have been stored.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanPageSizeEventsStoresOldestEventsInPreviousPage(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            UuidIri previousPageId =
                Guid.Parse(
                    writtenIndex.Links
                        .Single(AtomEventStream.IsPreviousFeedLink)
                        .Href.ToString());
            var actualPreviousPage = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == previousPageId);
            var expectedPreviousPage = new AtomFeedLikeness(
                before,
                previousPageId,
                events.AsEnumerable().Reverse().Skip(1).ToArray());
            Assert.True(
                expectedPreviousPage.Equals(actualPreviousPage),
                "Expected feed must match actual feed.");
        }

        [Theory, AutoAtomData]
        public void AppendAsyncExactlyTwicePageSizeEventsStoresTwoFeedPages(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var events = eventGenerator.Take(sut.PageSize * 2).ToList();
            events.ForEach(e => sut.AppendAsync(e).Wait());
            Assert.Equal(2, storage.Feeds.Count());
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanTwicePageSizeEventsCreatesThreeFeedPages(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var events = eventGenerator.Take(sut.PageSize * 2 + 1).ToList();
            events.ForEach(e => sut.AppendAsync(e).Wait());
            Assert.Equal(3, storage.Feeds.Count());
        }

        [Theory, AutoAtomData]
        public void AppendAsyncMoreThanTwicePageSizeEventAddsPreviousLinkToMiddlePage(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize * 2 + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            UuidIri previousPageId =
                Guid.Parse(
                    writtenIndex.Links
                        .Single(AtomEventStream.IsPreviousFeedLink)
                        .Href.ToString());
            var middlePage = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == previousPageId);
            Assert.Equal(
                1,
                middlePage.Links.Count(AtomEventStream.IsPreviousFeedLink));
        }

        [Theory, AutoAtomData]
        public void AppendAsyncTwicePageSizeEventDoesNotAddPreviousLinkToPreviousPage(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory storage,
            AtomFeedParser<XmlContentSerializer> parser,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var before = DateTimeOffset.Now;
            var events = eventGenerator.Take(sut.PageSize * 2).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var writtenIndex = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == sut.Id);
            UuidIri previousPageId =
                Guid.Parse(
                    writtenIndex.Links
                        .Single(AtomEventStream.IsPreviousFeedLink)
                        .Href.ToString());
            var previousPage = storage.Feeds
                .Select(parser.Parse)
                .Single(f => f.Id == previousPageId);
            Assert.Equal(
                0,
                previousPage.Links.Count(AtomEventStream.IsPreviousFeedLink));
        }

        [Theory, AutoAtomData]
        public void SutIsEnumerable(AtomEventStream<DataContractTestEventY> sut)
        {
            Assert.IsAssignableFrom<IEnumerable<DataContractTestEventY>>(sut);
        }

        [Theory, AutoAtomData]
        public void SutIsInitiallyEmpty(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<DataContractTestEventX> sut)
        {
            Assert.False(sut.Any(), "Intial event stream should be empty.");
            Assert.Empty(sut);
        }

        [Theory, AutoAtomData]
        public void SutYieldsCorrectEvents(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<XmlAttributedTestEventX> sut,
            List<XmlAttributedTestEventX> events)
        {
            events.ForEach(e => sut.AppendAsync(e).Wait());

            var expected = events.AsEnumerable().Reverse();
            Assert.True(
                expected.SequenceEqual(sut),
                "Events should be yielded in a FILO order");
            Assert.True(
                expected.Cast<object>().SequenceEqual(sut.OfType<object>()),
                "Events should be yielded in a FILO order");
        }

        [Theory, AutoAtomData]
        public void SutYieldsPagedEvents(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var events = eventGenerator.Take(sut.PageSize * 2 + 1).ToList();

            events.ForEach(e => sut.AppendAsync(e).Wait());

            var expected = events.AsEnumerable().Reverse();
            Assert.True(
                expected.SequenceEqual(sut),
                "Events should be yielded in a FILO order");
            Assert.True(
                expected.Cast<object>().SequenceEqual(sut.OfType<object>()),
                "Events should be yielded in a FILO order");
        }

        [Theory, AutoAtomData]
        public void SutCanAppendAndYieldPolymorphicEvents(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<IXmlAttributedTestEvent> sut,
            XmlAttributedTestEventX tex,
            XmlAttributedTestEventY tey)
        {
            sut.AppendAsync(tex).Wait();
            sut.AppendAsync(tey).Wait();

            var expected = new IXmlAttributedTestEvent[] { tey, tex };
            Assert.True(expected.SequenceEqual(sut));
        }

        [Theory, AutoAtomData]
        public void SutCanAppendAndYieldEnclosedPolymorphicEvents(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<DataContractEnvelope<IDataContractTestEvent>> sut,
            DataContractEnvelope<DataContractTestEventX> texEnvelope,
            DataContractEnvelope<DataContractTestEventY> teyEnvelope)
        {
            var texA = texEnvelope.Cast<IDataContractTestEvent>();
            var teyA = teyEnvelope.Cast<IDataContractTestEvent>();

            sut.AppendAsync(texA).Wait();
            sut.AppendAsync(teyA).Wait();

            var expected = new DataContractEnvelope<IDataContractTestEvent>[]
            {
                teyA,
                texA 
            };
            Assert.True(expected.SequenceEqual(sut));
        }

        [Theory, AutoAtomData]
        public void SutIsObserver(AtomEventStream<DataContractTestEventX> sut)
        {
            Assert.IsAssignableFrom<IObserver<DataContractTestEventX>>(sut);
        }

        [Theory, AutoAtomData]
        public void OnNextAppendsItem(
            [Frozen(Matching.ImplementedInterfaces)]AtomEventsInMemory dummyInjectedIntoSut,
            AtomEventStream<XmlAttributedTestEventX> sut,
            XmlAttributedTestEventX tex)
        {
            sut.OnNext(tex);
            Assert.Equal(tex, sut.SingleOrDefault());
        }

        [Theory, AutoAtomData]
        public void OnErrorDoesNotThrow(
            AtomEventStream<DataContractTestEventY> sut,
            Exception e)
        {
            sut.OnError(e);
        }

        [Theory, AutoAtomData]
        public void OnCompletedDoesNotThrow(
            AtomEventStream<DataContractTestEventY> sut)
        {
            sut.OnCompleted();
        }

        [Theory, AutoAtomData]
        public void AppendAsyncWritesPreviousPageBeforeIndex(
            [Frozen(Matching.ImplementedInterfaces)]SpyAtomEventStore spyStore,
            AtomEventStream<XmlAttributedTestEventX> sut,
            Generator<XmlAttributedTestEventX> eventGenerator)
        {
            var events = eventGenerator.Take(sut.PageSize + 1).ToList();
            
            events.ForEach(e => sut.AppendAsync(e).Wait());

            var feed = Assert.IsAssignableFrom<AtomFeed>(
                spyStore.ObservedArguments.Last());
            Assert.Equal(sut.Id, feed.Id);
        }
#pragma warning restore 618
    }
}
