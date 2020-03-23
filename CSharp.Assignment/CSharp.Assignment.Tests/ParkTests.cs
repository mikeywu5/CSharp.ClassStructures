using System;
using System.Reflection;
using CSharp.Assignments.Tests.Library;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Text;

namespace CSharp.Assignments.Classes.Structure.Tests
{

    public class ParkTests
    {
        [Test]
        public void TestFacilityType()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<FacilityType>();
            assert.Enum();
            string[] actual = assert.Type.GetEnumNames();
            string[] expected = { "National", "State", "Local" };
            CollectionAssert.AreEquivalent(expected, actual, $"The FacilityType Enum must consists of the following names: {string.Join(", ", expected)}");
#if !DEBUG
        });
#endif
        }

        [Test]
        public void TestParkClass()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Park>();
            assert.Class();
            assert.NonAbstract();
            assert.Field<string>(
                "_name",
                BindingFlags.Instance |
                BindingFlags.NonPublic
            ).Private().ReadOnly();

            assert.Field<string>(
               "_address",
               BindingFlags.Instance |
               BindingFlags.NonPublic
            ).Private().ReadOnly();

            assert.Field<FacilityType>(
               "_type",
               BindingFlags.Instance |
               BindingFlags.NonPublic
            ).Private().ReadOnly();

            assert.Field<string>(
               "_phone",
               BindingFlags.Instance |
               BindingFlags.NonPublic
            ).Private().ReadOnly();

            assert.Field<int>(
               "_openingHour",
               BindingFlags.Instance |
               BindingFlags.NonPublic
           ).Private().ReadOnly();

            assert.Field<int>(
               "_closingHour",
               BindingFlags.Instance |
               BindingFlags.NonPublic
           ).Private().ReadOnly();

            assert.Field<decimal>(
               "_fee",
               BindingFlags.Instance |
               BindingFlags.NonPublic
           ).Private().ReadOnly();

            assert.Constructor(
                BindingFlags.Public |
                BindingFlags.Instance,
                new Param<string>("name"),
                new Param<string>("address"),
                new Param<FacilityType>("type"),
                new Param<string>("phone"),
                new Param<int>("openingHour"),
                new Param<int>("closingHour"),
                new Param<decimal>("fee")
            );

            assert.Property<string>(
                "Info",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty
            );

            assert.Property<string>(
                "Contact",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty
            );

            assert.Method<decimal>(
                "CalculateFee",
                BindingFlags.Public |
                BindingFlags.Instance,
                new Param<int>("numberOfVisitors")
            );

            assert.Method<decimal>(
                "CalculateFee",
                BindingFlags.Public |
                BindingFlags.Static,
                new Param<int>("numberOfVisitors"),
                new Param<Park[]>("parks")
            );

            assert.Method(
                "Show",
                BindingFlags.Public |
                BindingFlags.Static,
                new Param<Park[]>("parks")
            );
#if !DEBUG
    });
#endif
        }
        [Test]
        public void TestParkInstanceMethods()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var park = new TypeAssert<Park>();
            dynamic actual = park.New(
                "Park 1",
                "123 Test Drive",
                (FacilityType)Enum.Parse(typeof(FacilityType), "National"),
                "444-432-9876", 5, 18, 12.34m);
            Assert.AreEqual("Park 1 National Park 5 AM to 6 PM $12.34", actual.Info);
            Assert.AreEqual("444-432-9876 123 Test Drive", actual.Contact);
            Assert.AreEqual(419.56m, actual.CalculateFee(34));

            park.Catch<ArgumentOutOfRangeException>(
                () => actual = park.New(
                "Park 1",
                "123 Test Drive",
                (FacilityType)Enum.Parse(typeof(FacilityType), "National"),
                "444-432-9876", 6, 5, 12.34m)
            );
#if !DEBUG
});
#endif
        }

        [Test]
        public void TestParkClassMethods()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var park = new TypeAssert<Park>();
            Park[] parks = new Park[] {
                park.New(
               "Park 1",
               "123 Test Drive",
               (FacilityType)Enum.Parse(typeof(FacilityType), "National"),
               "444-432-9876", 5, 11, 4.56m),
                park.New(
               "Park 2",
               "1 Park Street",
               (FacilityType)Enum.Parse(typeof(FacilityType), "Local"),
               "777-888-3332", 13, 18, 7.89m),
                park.New(
               "Park 3",
               "1 Midnight Lane",
               (FacilityType)Enum.Parse(typeof(FacilityType), "State"),
               "893-221-1234", 0, 12, 12.34m)
            };
            var show = park.Method(
                "Show",
                BindingFlags.Public |
                BindingFlags.Static,
                new Param<Park[]>("parks")
            );

            var calculate = park.Method<decimal>(
                "CalculateFee",
                BindingFlags.Public |
                BindingFlags.Static,
                new Param<int>("numberOfVisitors"),
                new Param<Park[]>("parks")
            );
            // Park.Show(parks);
            Action call = () => show.Invoke(null, new object[] { parks });
            string actual = call.Run();
            actual.Assert(
                "Park 1 National Park 5 AM to 11 AM $4.56 444-432-9876 123 Test Drive",
                "Park 2 Local Park 1 PM to 6 PM $7.89 777-888-3332 1 Park Street",
                "Park 3 State Park 12 AM to 12 PM $12.34 893-221-1234 1 Midnight Lane"
            );
            // decimal actualFee = Park.CalculateFee(765, parks);
            decimal actualFee = (decimal)calculate.Invoke(null, new object[] { 765, parks });
            Assert.AreEqual(18964.35m, actualFee);
#if !DEBUG
});
#endif
        }
    }
}
