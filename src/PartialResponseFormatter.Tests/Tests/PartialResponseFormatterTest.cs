using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class PartialResponseFormatterTest
    {
        private PartialResponseFormatter formatter;

        [SetUp]
        public void SetUp()
        {
            formatter = new PartialResponseFormatter();
        }

        [Test]
        public void TestFormatInt()
        {
            const int expected = 5;
            var actual = formatter.Format(expected, ResponseSpecification.Empty);
            actual.Should().Be(expected);
        }

        [Test]
        public void TestFormatString()
        {
            const string expected = "abacaba";
            var actual = formatter.Format(expected, ResponseSpecification.Empty);
            actual.Should().Be(expected);
        }

        [Test]
        public void TestFormatNull()
        {
            var actual = formatter.Format(null, ResponseSpecification.Empty);
            actual.Should().BeNull();
        }

        [Test]
        public void TestFormatEmptyArray()
        {
            var actual = formatter.Format(new string[0], ResponseSpecification.Empty);
            (actual as IEnumerable).Should().BeEmpty();
        }

        [Test]
        public void TestFormatArray()
        {
            var array = new[] {new {A = "a", B = "b"}, new {A = "aa", B = "bb"}};
            var actual = formatter.Format(array, ResponseSpecification.Field("A"));
            var expected = new[] {new {A = "a"}, new {A = "aa"}};
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatEmptyDictionary()
        {
            var actual = formatter.Format(new Dictionary<string, string>(), ResponseSpecification.Empty);
            (actual as IDictionary).Should().BeEmpty();
        }

        public class ABClass
        {
            public string A { get; set; }
            public string B { get; set; }
        }
        
        public class AClass
        {
            public string A { get; set; }
        }
        
        [Test]
        public void TestFormatDictionary()
        {
            var dictionary = new Dictionary<string, ABClass>
            {
                {"item1", new ABClass{A = "a", B = "b"}},
                {"item2", new ABClass{A = "aa", B = "bb"}}
            };
            var actual = formatter.Format(dictionary, ResponseSpecification.Field("a"));
            var expected = new Dictionary<string, AClass>
            {
                {"item1", new AClass{A = "a"}},
                {"item2", new AClass{A = "aa"}}
            };
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatPropertyNameCaseNotMatters()
        {
            var array = new {A = "a"};
            var actual = formatter.Format(array, ResponseSpecification.Field("a"));
            var expected = new {a = "a"};
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatNested()
        {
            var array = new {A = "a", B = "b", C = new {D = "d", E = "e"}};
            var actual = formatter.Format(array,
                ResponseSpecification
                    .Field("A")
                    .Field("C",
                        _ => _.Field("E"))
            );
            var expected = new {A = "a", C = new {E = "e"}};
            AssertExpectedContract(actual, expected);
        }

        public class TestContract
        {
            public string A { get; set; }
            public int B { get; set; }
            public DateTime C { get; set; }
            public decimal D { get; set; }
            public NestedTestContract Nested { get; set; }
            public NestedTestContract[] Array { get; set; }
            public List<NestedTestContract> List { get; set; }
            public Dictionary<string, NestedTestContract> Dictionary { get; set; }
        }

        public class NestedTestContract
        {
            public string X { get; set; }
            public int Y { get; set; }
            public DateTime Z { get; set; }
        }

        [Test]
        public void TestFormatComplexObject()
        {
            var nested1 = new NestedTestContract
            {
                X = "xxx",
                Y = 666,
                Z = new DateTime(2099, 01, 01)
            };
            var nested2 = new NestedTestContract
            {
                X = "yyy",
                Y = 777,
                Z = new DateTime(9999, 01, 01)
            };
            var testContract = new TestContract
            {
                A = "aaa",
                B = 42,
                C = new DateTime(2016, 02, 10),
                D = 36.6m,
                Nested = nested1,
                Array = new[] {nested1, nested2},
                List = new List<NestedTestContract> {nested1, nested2},
                Dictionary = new Dictionary<string, NestedTestContract>
                {
                    {"item1", nested1},
                    {"item2", nested2}
                }
            };

            var nestedFields = ResponseSpecification.Field("X").Field("Z");
            var responseSpecification = ResponseSpecification
                .Field("A")
                .Field("D")
                .Field("C")
                .Field("Array", _ => _
                    .Field("X")
                    .Field("Z")
                )
                .Field("List", _ => _
                    .Field("X")
                    .Field("Z")
                )
                .Field("Nested", _ => nestedFields)
                .Field("Dictionary", _ => nestedFields);

            var actual = formatter.Format(testContract, responseSpecification);

            var nestedExpected1 = new
            {
                X = "xxx",
                Z = new DateTime(2099, 01, 01)
            };
            var nestedExpected2 = new
            {
                X = "yyy",
                Z = new DateTime(9999, 01, 01)
            };
            var expected = new
            {
                A = "aaa",
                D = 36.6,
                C = new DateTime(2016, 02, 10),
                Array = new[]
                {
                    nestedExpected1,
                    nestedExpected2,
                },
                List = new[]
                {
                    nestedExpected1,
                    nestedExpected2,
                },
                Nested = nestedExpected1,
                Dictionary = new
                {
                    item1 = nestedExpected1,
                    item2 = nestedExpected2
                }
            };
            AssertExpectedContract(actual, expected);
        }

        private void AssertExpectedContract(object actual, object expected)
        {
            var actualSerialization = JsonConvert.SerializeObject(actual, Formatting.Indented);
            var expectedSerialization = JsonConvert.SerializeObject(expected, Formatting.Indented);
            Console.WriteLine(actualSerialization);
            actualSerialization.ShouldBeEquivalentTo(expectedSerialization);
        }
    }
}