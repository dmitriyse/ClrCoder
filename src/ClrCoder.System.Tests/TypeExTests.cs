using FluentAssertions;

using NUnit.Framework;

namespace ClrCoder.System.Tests
{
    /// <summary>
    /// Tests for <see cref="TypeEx"/> and <see cref="TypeEx{T}"/> classes.
    /// </summary>
    [TestFixture]
    public class TypeExTests
    {
        /// <summary>
        /// Test for the <see cref="TypeEx{T}.CreateConstructorDelegate"/> method.
        /// </summary>
        [Test]
        public void CreateConstructorDelegateClass()
        {
            var objConstructor = TypeEx<object>.CreateConstructorDelegate();
            var obj = objConstructor.Invoke();

            obj.Should().NotBeNull();
        }
    }
}