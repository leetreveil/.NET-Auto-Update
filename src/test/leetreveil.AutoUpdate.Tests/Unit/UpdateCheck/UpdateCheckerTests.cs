using System;
using NUnit.Framework;
using leetreveil.AutoUpdate.Core.UpdateCheck;

namespace leetreveil.AutoUpdate.Tests.Unit.UpdateCheck
{
    [TestFixture]
    public class UpdateCheckerTests
    {
        [Test]
        public void Should_not_find_an_update_when_the_version_is_the_same_as_the_one_specified_in_the_update()
        {
            var result = UpdateChecker.CheckForUpdate(new Version(1, 0,0), new Version(1, 0));

            Assert.That(result,Is.False);
        }

        [Test]
        public void Should_not_find_an_update_when_the_version_is_less_than_as_the_one_specified_in_the_update()
        {
            var result = UpdateChecker.CheckForUpdate(new Version(1, 1), new Version(1, 0));

            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_find_an_update_when_the_version_is_greater_than_as_the_one_specified_in_the_update()
        {
            var result = UpdateChecker.CheckForUpdate(new Version(1, 0), new Version(1, 1));

            Assert.That(result, Is.True);
        }
    }
}