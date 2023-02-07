﻿using System;
using System.Linq;

namespace Grean.AtomEventStore.UnitTests
{
    public class Conventions
    {
        [Theory, AutoAtomData]
        public void VerifyGuardClauses(GuardClauseAssertion assertion)
        {
            var representative = typeof(AtomEventStream);
            var targets = from t in representative.Assembly.GetExportedTypes()
                          where Include(t)
                          from m in t.GetMembers()
                          where m.Name != "TryParse"
                          select m;

            assertion.Verify(targets);
        }

        private static bool Include(Type t)
        {
#pragma warning disable 618
            return !t.IsInterface
                && !IsStatic(t)
                && t != typeof(AtomLink)             // Covered by AtomLinkTests
                && t != typeof(AtomAuthor)           // Covered by AtomAuthorTests
                && t != typeof(AtomEventStream<>)    // Covered by AtomEventStreamTests
                && t != typeof(AtomEventObserver<>); // Covered by AtomEventObserverTests
#pragma warning restore 618
        }

        private static bool IsStatic(Type t)
        {
            return t.IsSealed && t.IsAbstract;
        }
    }
}
