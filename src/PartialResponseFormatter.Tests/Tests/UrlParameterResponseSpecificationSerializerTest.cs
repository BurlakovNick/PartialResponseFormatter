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
        public void Test_Serialize_And_Deserialize(ResponseSpecification expectedSpecification,
            string expectedSerialization)
        {
            var serializer = new UrlParameterResponseSpecificationSerializer();

            var actualSerialization = serializer.Serialize(expectedSpecification);
            Console.WriteLine($"Actual serialization: {actualSerialization}");
            actualSerialization.ShouldBeEquivalentTo(expectedSerialization);

            var actualSpecification = serializer.Deserialize(expectedSerialization);
            Console.WriteLine(
                $"Actual specification: {JsonConvert.SerializeObject(actualSpecification, Formatting.Indented)}");
            actualSpecification.ShouldBeEquivalentTo(expectedSpecification);
        }

        private static IEnumerable<TestCaseData> GenerateTestData()
        {
            yield return new TestCaseData(
                ResponseSpecification.Empty,
                string.Empty
            ).SetName("Empty");

            yield return new TestCaseData(
                ResponseSpecification.Field("name").Create(),
                "name"
            ).SetName("Single field");

            yield return new TestCaseData(
                ResponseSpecification
                    .Field("name1")
                    .Field("name2")
                    .Create(),
                "name1;name2"
            ).SetName("Two fields");

            yield return new TestCaseData(
                ResponseSpecification
                    .Field("name1", _ => _.Field("a").Field("b"))
                    .Field("name2", _ => _.Field("d").Field("a"))
                    .Create(),
                "name1:(a;b);name2:(d;a)"
            ).SetName("Two fields with nested fields");

            yield return new TestCaseData(
                ResponseSpecification
                    .Field(
                        "root", root => root
                            .Field(
                                "l", l => l
                                    .Field(
                                        "ll", ll => ll
                                            .Field("lll")
                                            .Field("llr")
                                    )
                                    .Field(
                                        "lr", lr => lr
                                            .Field("lrl")
                                            .Field("lrr")
                                    )
                            )
                            .Field(
                                "r", r => r
                                    .Field(
                                        "rl", ll => ll
                                            .Field("rll")
                                            .Field("rlr")
                                    )
                                    .Field(
                                        "rr", lr => lr
                                            .Field("rrl")
                                            .Field("rrr")
                                    )
                            )
                    ).Create(),
                "root:(l:(ll:(lll;llr);lr:(lrl;lrr));r:(rl:(rll;rlr);rr:(rrl;rrr)))"
            ).SetName("Binary tree");
        }
    }
}