using System;
using FluentAssertions;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class DelegateGeneratorTest
    {
        public class TestClass
        {
            public string A { get; set; }
            public NestedClass B { get; set; }
            public DateTime C { get; set; }
            public decimal D { get; set; }
            public double E { get; set; }
            public int F { get; set; }
            public Guid G { get; set; }
            public string[] H { get; set; }
        }
        
        public class NestedClass
        {
            public string Data { get; set; }
        }
        
        [Test]
        public void TestGeneratePropertyGetter()
        {
            var actual = new TestClass
            {
                A = "aaa",
                B = new NestedClass {Data = "data"},
                C = DateTime.Now,
                D = 42m,
                E = 54.0,
                F = 666,
                G = Guid.NewGuid(),
                H = new [] { "a", "b", "c"}
            };

            var aGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("A"));
            var bGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("B"));
            var cGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("C"));
            var dGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("D"));
            var eGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("E"));
            var fGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("F"));
            var gGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("G"));
            var hGetter = DelegateGenerator.GeneratePropertyGetter(actual.GetType().GetProperty("H"));

            aGetter(actual).Should().Be(actual.A);
            bGetter(actual).Should().Be(actual.B);
            cGetter(actual).Should().Be(actual.C);
            dGetter(actual).Should().Be(actual.D);
            eGetter(actual).Should().Be(actual.E);
            fGetter(actual).Should().Be(actual.F);
            gGetter(actual).Should().Be(actual.G);
            hGetter(actual).Should().Be(actual.H);
        }
    }
}