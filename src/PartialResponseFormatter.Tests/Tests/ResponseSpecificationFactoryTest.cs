using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class ResponseSpecificationFactoryTest
    {
        private ResponseSpecificationFactory responseSpecificationFactory;

        [SetUp]
        public void SetUp()
        {
            responseSpecificationFactory = new ResponseSpecificationFactory();
        }
        
        public class SimpleType
        {
            public string A { get; set; }
        }
        
        [Test]
        public void TestCreateForSimpleType()
        {
            var actual = responseSpecificationFactory.Create<SimpleType>();
            actual.ShouldBeEquivalentTo(new ResponseSpecification
            {
                Fields = new[]
                {
                    new Field {Name = "A",}
                }
            });
        }
        
        public class ComplexType
        {
            public string A { get; set; }
            //public DateTime[] B { get; set; }
            public Dictionary<int, NestedType> C { get; set; }
            public NestedType D { get; set; }
            public NestedType[] E { get; set; }
        }
        
        public class NestedType
        {
            public int X { get; set; }
        }
        
        [Test]
        public void TestCreateForComplexType()
        {
            var actual = responseSpecificationFactory.Create<ComplexType>();
            actual.ShouldBeEquivalentTo(new ResponseSpecification
            {
                Fields = new[]
                {
                    new Field
                    {
                        Name = "A",
                    },
                    new Field
                    {
                        Name = "C",
                        Fields = new[]
                        {
                            new Field {Name = "X"},
                        }
                    },
                    new Field
                    {
                        Name = "D",
                        Fields = new[]
                        {
                            new Field {Name = "X"},
                        }
                    },
                    new Field
                    {
                        Name = "E",
                        Fields = new[]
                        {
                            new Field {Name = "X"},
                        }
                    },
                    
                }
            });
        }
    }
}