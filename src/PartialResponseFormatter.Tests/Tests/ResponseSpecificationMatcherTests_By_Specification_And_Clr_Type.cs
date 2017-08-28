using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class ResponseSpecificationMatcherTests_By_Specification_And_Clr_Type
    {
        [Test]
        public void Test_Empty_Fields_And_Int() => CheckMatches<int>(ResponseSpecification.Empty);

        [Test]
        public void Test_Empty_Fields_And_Array() => CheckMatches<int[]>(ResponseSpecification.Empty);

        [Test]
        public void Test_Empty_Fields_And_Dictionary() =>
            CheckMatches<Dictionary<int, int>>(ResponseSpecification.Empty);

        [Test]
        public void Test_Single_Field_And_Int() => CheckNotMatches<int>(ResponseSpecification.Field("A"), "root.A");

        [Test]
        public void Test_Single_Field_And_Array() =>
            CheckNotMatches<int[]>(ResponseSpecification.Field("A"), "root.[list].A");

        [Test]
        public void Test_Single_Field_And_Dictionary() =>
            CheckNotMatches<Dictionary<int, int>>(ResponseSpecification.Field("A"), "root.[dictionary].A");

        public class Simple_Class
        {
            public int A { get; set; }
            public string B { get; set; }
        }

        [Test]
        public void Test_Single_Field_Matches() => CheckMatches<Simple_Class>(ResponseSpecification.Field("A"));

        [Test]
        public void Test_Single_Field_And_Not_Matched_Object() =>
            CheckNotMatches<Simple_Class>(ResponseSpecification.Field("C"), "root.C");

        [Test]
        public void Test_Single_Field_Matches_Array_Of_Object() =>
            CheckMatches<Simple_Class[]>(ResponseSpecification.Field("A"));

        [Test]
        public void Test_Single_Field_Matches_Dictionary_Of_Object() =>
            CheckMatches<Dictionary<string, Simple_Class>>(ResponseSpecification.Field("A"));

        public class Complex_Class
        {
            public string X { get; set; }
            public Dictionary<string, Simple_Class> Y { get; set; }
            public Simple_Class[] Z { get; set; }
        }
        
        [Test]
        public void Test_Single_Field_Not_Matches_Complex_Object() =>
            CheckNotMatches<Complex_Class>(ResponseSpecification.Field("A"), "root.A");

        [Test]
        public void Test_Full_Match_Complex_Object() =>
            CheckMatches<Complex_Class>(
                ResponseSpecification
                    .Field("X")
                    .Field("Y", _ => _.Field("A"))
                    .Field("Z", _ => _.Field("B"))
            );

        [Test]
        public void Test_Not_Matches_Complex_Object() =>
            CheckNotMatches<Complex_Class>(
                ResponseSpecification
                    .Field("A")
                    .Field("Y", _ => _.Field("C").Field("B"))
                    .Field("Z", _ => _.Field("A").Field("D")),
                "root.A",
                "root.Y.[dictionary].C",
                "root.Z.[list].D"
            );
        
        private static void CheckMatches<TServerData>(ResponseSpecification clientResponseSpecification)
        {
            FieldMismatch[] actual;
            ResponseSpecification.CheckClientMatchesServer<TServerData>(clientResponseSpecification, out actual)
                .Should().BeTrue();
            actual.Should().BeEmpty();
        }

        private static void CheckNotMatches<TServerData>(ResponseSpecification clientResponseSpecification,
            params string[] paths)
        {
            FieldMismatch[] actual;
            ResponseSpecification.CheckClientMatchesServer<TServerData>(clientResponseSpecification, out actual)
                .Should().BeFalse();
            var expected = paths.Select(p => new FieldMismatch(p, string.Empty)).ToArray();
            actual.ShouldAllBeEquivalentTo(expected, opt => opt.Excluding(x => x.Message));
        }
    }
}