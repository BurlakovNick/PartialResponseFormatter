using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace RestPartialResponse.Test.Tests
{
    [TestFixture]
    public class PartialResponseFormatterTest
    {
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
        public void TestFormat()
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
                Array = new [] { nested1, nested2},
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
            
            var formatter = new PartialResponseFormatter();
            var actual = formatter.Format(testContract, responseSpecification);
            Console.WriteLine(JsonConvert.SerializeObject(actual, Formatting.Indented));
        }
    }
}