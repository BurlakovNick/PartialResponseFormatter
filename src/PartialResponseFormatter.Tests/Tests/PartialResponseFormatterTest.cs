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
            var actual = formatter.Format(expected, new ResponseSpecification {Fields = new Field[0]});
            actual.Should().Be(expected);
        }

        [Test]
        public void TestFormatString()
        {
            const string expected = "abacaba";
            var actual = formatter.Format(expected, new ResponseSpecification {Fields = new Field[0]});
            actual.Should().Be(expected);
        }

        [Test]
        public void TestFormatNull()
        {
            var actual = formatter.Format(null, new ResponseSpecification {Fields = new Field[0]});
            actual.Should().BeNull();
        }

        [Test]
        public void TestFormatEmptyArray()
        {
            var actual = formatter.Format(new string[0], new ResponseSpecification {Fields = new Field[0]});
            (actual as IEnumerable).Should().BeEmpty();
        }

        [Test]
        public void TestFormatArray()
        {
            var array = new[] {new {A = "a", B = "b"}, new {A = "aa", B = "bb"}};
            var actual = formatter.Format(array, new ResponseSpecification {Fields = new[] {new Field {Name = "A"},}});
            var expected = new[] {new {A = "a"}, new {A = "aa"}};
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatEmptyDictionary()
        {
            var actual = formatter.Format(new Dictionary<string, string>(),
                new ResponseSpecification {Fields = new Field[0]});
            (actual as IDictionary).Should().BeEmpty();
        }

        [Test]
        public void TestFormatDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"item1", new {A = "a", B = "b"}},
                {"item2", new {A = "aa", B = "bb"}}
            };
            var actual = formatter.Format(dictionary,
                new ResponseSpecification {Fields = new[] {new Field {Name = "A"},}});
            var expected = new Dictionary<string, object>
            {
                {"item1", new {A = "a"}},
                {"item2", new {A = "aa"}}
            };
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatPropertyNameCaseNotMatters()
        {
            var array = new {A = "a"};
            var actual = formatter.Format(array, new ResponseSpecification {Fields = new[] {new Field {Name = "a"},}});
            var expected = new {a = "a"};
            AssertExpectedContract(actual, expected);
        }

        [Test]
        public void TestFormatNested()
        {
            var array = new {A = "a", B = "b", C = new {D = "d", E = "e"}};
            var actual = formatter.Format(array, new ResponseSpecification
            {
                Fields = new[]
                {
                    new Field {Name = "A"},
                    new Field {Name = "C", Fields = new[] {new Field {Name = "E"},}}
                }
            });
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
            var nestedFields = new Field[]
            {
                new Field
                {
                    Name = "X"
                },
                new Field
                {
                    Name = "Z"
                }
            };
            var responseSpecification = new ResponseSpecification
            {
                Fields = new Field[]
                {
                    new Field
                    {
                        Name = "A"
                    },
                    new Field
                    {
                        Name = "D"
                    },
                    new Field
                    {
                        Name = "C"
                    },
                    new Field
                    {
                        Name = "Array",
                        Fields = nestedFields
                    },
                    new Field
                    {
                        Name = "List",
                        Fields = nestedFields
                    },
                    new Field
                    {
                        Name = "Nested",
                        Fields = nestedFields
                    },
                    new Field
                    {
                        Name = "Dictionary",
                        Fields = nestedFields
                    },
                }
            };

            var actual = formatter.Format(testContract, responseSpecification);
            var actualSerialization = JsonConvert.SerializeObject((object) actual, Formatting.Indented);

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