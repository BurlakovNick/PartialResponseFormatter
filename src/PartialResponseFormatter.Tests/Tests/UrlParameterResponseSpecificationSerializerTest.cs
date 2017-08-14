using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class UrlParameterResponseSpecificationSerializerTest
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestData))]
        public void TestSerializeAndDeserialize(Field[] fields, string expectedSerialization)
        {
            var expectedSpecification = new ResponseSpecification { Fields = fields};
            var serializer = new UrlParameterResponseSpecificationSerializer();

            var actualSerialization = serializer.Serialize(expectedSpecification);
            Console.WriteLine($"Actual serialization: {actualSerialization}");
            actualSerialization.ShouldBeEquivalentTo(expectedSerialization);

            var actualSpecification = serializer.Deserialize(expectedSerialization);
            Console.WriteLine($"Actual specification: {JsonConvert.SerializeObject(actualSpecification, Formatting.Indented)}");
            actualSpecification.ShouldBeEquivalentTo(expectedSpecification);
        }

        private static IEnumerable<TestCaseData> GenerateTestData()
        {
            yield return new TestCaseData(
                new Field[0],
                string.Empty
            ).SetName("Empty");
            
            yield return new TestCaseData(
                new []
                {
                    new Field { Name = "name" },
                },
                "name"
            ).SetName("Single field");
            
            yield return new TestCaseData(
                new []
                {
                    new Field { Name = "name1" },
                    new Field { Name = "name2" },
                },
                "name1;name2"
            ).SetName("Two fields");
            
            yield return new TestCaseData(
                new []
                {
                    new Field { Name = "name1", Fields = new [] { new Field {Name = "a"}, new Field {Name = "b"} }},
                    new Field { Name = "name2", Fields = new [] { new Field {Name = "d"}, new Field {Name = "a"} }},
                },
                "name1:(a;b);name2:(d;a)"
            ).SetName("Two fields with nested fields");
            
            yield return new TestCaseData(
                new []
                {
                    new Field
                    {
                        Name = "root",
                        Fields = new []
                        {
                            new Field
                            {
                                Name = "l",
                                Fields = new []
                                {
                                    new Field
                                    {
                                        Name = "ll",
                                        Fields = new []
                                        {
                                            new Field
                                            {
                                                Name = "lll"
                                            },
                                            new Field
                                            {
                                                Name = "llr"
                                            },
                                        }
                                    },
                                    new Field
                                    {
                                        Name = "lr",
                                        Fields = new []
                                        {
                                            new Field
                                            {
                                                Name = "lrl"
                                            },
                                            new Field
                                            {
                                                Name = "lrr"
                                            },
                                        }
                                    }
                                }
                            },
                            new Field
                            {
                                Name = "r",
                                Fields = new []
                                {
                                    new Field
                                    {
                                        Name = "rl",
                                        Fields = new []
                                        {
                                            new Field
                                            {
                                                Name = "rll"
                                            },
                                            new Field
                                            {
                                                Name = "rlr"
                                            },
                                        }
                                    },
                                    new Field
                                    {
                                        Name = "rr",
                                        Fields = new []
                                        {
                                            new Field
                                            {
                                                Name = "rrl"
                                            },
                                            new Field
                                            {
                                                Name = "rrr"
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                "root:(l:(ll:(lll;llr);lr:(lrl;lrr));r:(rl:(rll;rlr);rr:(rrl;rrr)))"
            ).SetName("Binary tree");
        }
    }
}