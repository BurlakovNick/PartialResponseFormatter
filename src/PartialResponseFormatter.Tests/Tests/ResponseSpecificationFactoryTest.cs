using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class ResponseSpecificationFactoryTest
    {
        
        public class SimpleType
        {
            public string A { get; set; }
        }
        
        [Test]
        public void TestCreateForSimpleType()
        {
            var actual = ResponseSpecification.Create<SimpleType>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Field("A").Create());
        }
        
        public class ComplexType
        {
            public string A { get; set; }
            public DateTime[] B { get; set; }
            public Dictionary<int, NestedType> C { get; set; }
            public NestedType D { get; set; }
            public NestedType[] E { get; set; }
            public long F { get; set; }
            public decimal G { get; set; }
            public double H { get; set; }
            public Guid I { get; set; }
            public long? J { get; set; }
            public LessComplexType K { get; set; }
        }

        public class LessComplexType
        {
            public string W { get; set; }
            public NestedType Z { get; set; }
        }

        public class NestedType
        {
            public int X { get; set; }
        }

        [Test]
        public void TestCreateForComplexType()
        {
            var actual = ResponseSpecification.Create<ComplexType>();
            var expected = ResponseSpecification
                .Field("A")
                .Field("B")
                .Field("C", _ => _.Field("X"))
                .Field("D", _ => _.Field("X"))
                .Field("E", _ => _.Field("X"))
                .Field("F")
                .Field("G")
                .Field("H")
                .Field("I")
                .Field("J")
                .Field("K", _ => _
                    .Field("W")
                    .Field("Z", __ => __.Field("X"))
                )
                .Create();

            actual.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void TestInt()
        {
            var actual = ResponseSpecification.Create<int>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Empty);
        }

        [Test]
        public void TestDictionaryOfGuids()
        {
            var actual = ResponseSpecification.Create<Dictionary<Guid, Guid>>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Empty);
        }

        [Test]
        public void TestDictionaryOfNestedTypes()
        {
            var actual = ResponseSpecification.Create<Dictionary<Guid, NestedType>>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Field("X").Create());
        }

        [Test]
        public void TestDictionaryOfDictionaryOfNestedTypes()
        {
            var actual = ResponseSpecification.Create<Dictionary<Guid, Dictionary<string, NestedType>>>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Field("X").Create());
        }

        [Test]
        public void TestArrayOfInt()
        {
            var actual = ResponseSpecification.Create<int[]>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Empty);
        }

        [Test]
        public void TestArrayOfNestedTypes()
        {
            var actual = ResponseSpecification.Create<NestedType[]>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Field("X").Create());
        }

        [Test]
        public void TestArrayOfArrayOfNestedTypes()
        {
            var actual = ResponseSpecification.Create<NestedType[][]>();
            actual.ShouldBeEquivalentTo(ResponseSpecification.Field("X").Create());
        }
    }
}