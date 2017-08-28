using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.Tests
{
    [TestFixture]
    public class ResponseSpecificationMatcherTests_By_Clr_Types
    {
        [Test]
        public void Test_String_And_Int() => CheckMatches<string, int>();

        [Test]
        public void Test_Date_And_Date() => CheckMatches<DateTime, DateTime>();

        [Test]
        public void Test_Int_And_Array() => CheckNotMatches<int, int[]>("root");

        [Test]
        public void Test_Int_And_Dictionary() => CheckNotMatches<int, Dictionary<int, int>>("root");

        [Test]
        public void Test_Array_And_Dictionary() => CheckNotMatches<int[], Dictionary<int, int>>("root");

        [Test]
        public void Test_Array_And_Array() => CheckMatches<int[], string[]>();

        [Test]
        public void Test_Dictionary_And_Dictionary() =>
            CheckMatches<Dictionary<int, int>, Dictionary<string, string>>();

        public class Simple_Class_1
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        public class Simple_Class_2
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        public class Simple_Class_Without_One_Property
        {
            public string A { get; set; }
        }

        [Test]
        public void Test_Two_Equal_Classes() => CheckMatches<Simple_Class_1, Simple_Class_1>();

        [Test]
        public void Test_Two_Equivalent_Classes() => CheckMatches<Simple_Class_1, Simple_Class_2>();

        [Test]
        public void Test_Two_Almost_Equivalent_Classes_When_Server_Has_No_Field() =>
            CheckNotMatches<Simple_Class_1, Simple_Class_Without_One_Property>("root.B");

        [Test]
        public void Test_Two_Almost_Equivalent_Classes_When_Client_Has_No_Field() =>
            CheckMatches<Simple_Class_Without_One_Property, Simple_Class_1>();

        public class Complex_Class_1
        {
            public string A { get; set; }
            public Dictionary<string, Dictionary<string, string>> B { get; set; }
            public string[][] C { get; set; }
            public Nested_Class_1[] D { get; set; }
        }

        public class Nested_Class_1
        {
            public string X { get; set; }
            public string Y { get; set; }
            public Dictionary<string, string> Z { get; set; }
        }

        public class Complex_Class_2
        {
            public string AA { get; set; }
            public Dictionary<string, int[]> B { get; set; }
            public int[] C { get; set; }
            public Nested_Class_2[] D { get; set; }
        }

        public class Nested_Class_2
        {
            public string X { get; set; }
            public string Z { get; set; }
            public string WTF { get; set; }
        }

        [Test]
        public void Test_Two_Complex_Objects() =>
            CheckNotMatches<Complex_Class_1, Complex_Class_2>(
                "root.A",
                "root.B.[dictionary]",
                "root.C.[list]",
                "root.D.[list].Y",
                "root.D.[list].Z"
            );

        private static void CheckMatches<T1, T2>()
        {
            FieldMismatch[] actual;
            ResponseSpecification.CheckClientMatchesServer<T1, T2>(out actual).Should().BeTrue();
            actual.Should().BeEmpty();
        }

        private static void CheckNotMatches<T1, T2>(params string[] paths)
        {
            FieldMismatch[] actual;
            ResponseSpecification.CheckClientMatchesServer<T1, T2>(out actual).Should().BeFalse();
            var expected = paths.Select(p => new FieldMismatch(p, string.Empty)).ToArray();
            actual.ShouldAllBeEquivalentTo(expected, opt => opt.Excluding(x => x.Message));
        }
    }
}