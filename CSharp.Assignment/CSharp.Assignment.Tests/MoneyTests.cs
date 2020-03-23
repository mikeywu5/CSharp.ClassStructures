using System;
using NUnit.Framework;
using CSharp.Assignments.Tests.Library;
using System.Reflection;

namespace CSharp.Assignments.Classes.Structure.Tests
{
    public class MoneyTests
    {
        [Test]
        public void Money()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Money>();
            assert.Constructor(
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<decimal>("money")
            );
            assert.Constructor(
                BindingFlags.Instance |
                 BindingFlags.Public,
                new Param<Money>("money")
            );
            assert.Constructor(
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<int>("dollars"),
                new Param<int>("cents")
            );
            assert.Property<int>(
                "Dollars",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty
            );
            assert.Property<int>(
                "Cents",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty
            );
            assert.Method(
                "IncrementMoney",
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<Money>("money")
            );
            assert.Method(
                "DecrementMoney",
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<Money>("money")
            );

            assert.Method<string>(
                "ToString",
                BindingFlags.Instance |
                BindingFlags.Public
            ).Override();
#if !DEBUG
        });
#endif
        }

        [Test]
        public void NewMoney()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Money>();
            dynamic money;
            // money = new Money(12, 34);
            money = assert.New(12, 34);
            Assert.AreEqual(12, money.Dollars);
            Assert.AreEqual(34, money.Cents);
            Assert.AreEqual($"{12.34:C}", money.ToString());
            assert.Catch<ArgumentOutOfRangeException>(() => money = assert.New(-1, 99));
#if !DEBUG
    });
#endif
        }

        [Test]
        public void IncrementDecrementMoney()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Money>();
            dynamic money, added, minus;
            money = assert.New(12, 34);
            added = assert.New(56, 68);
            money.IncrementMoney(added);
            Assert.AreEqual(69, money.Dollars);
            Assert.AreEqual(2, money.Cents);
            Assert.AreEqual("$69.02", money.ToString());
            Assert.AreEqual(56, added.Dollars);
            Assert.AreEqual(68, added.Cents);
            Assert.AreEqual("$56.68", added.ToString());

            money = assert.New(12, 34);
            minus = assert.New(3, 35);
            money.DecrementMoney(minus);
            Assert.AreEqual(8, money.Dollars);
            Assert.AreEqual(99, money.Cents);
            Assert.AreEqual("$8.99", money.ToString());
            Assert.AreEqual(3, minus.Dollars);
            Assert.AreEqual(35, minus.Cents);
            Assert.AreEqual("$3.35", minus.ToString());

            money = assert.New(12, 34);
            minus = assert.New(12, 35);
            assert.Catch<ArgumentOutOfRangeException>(() => money.DecrementMoney(minus));

            Assert.AreEqual(12, money.Dollars);
            Assert.AreEqual(34, money.Cents);
            Assert.AreEqual("$12.34", money.ToString());
            Assert.AreEqual(12, minus.Dollars);
            Assert.AreEqual(35, minus.Cents);
            Assert.AreEqual("$12.35", minus.ToString());
#if !DEBUG
});
#endif
        }

        [Test]
        public void DollarsCents()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Money>();
            dynamic money = Activator.CreateInstance(typeof(Money), 0, 0);
            Assert.AreEqual(0, money.Dollars);
            Assert.AreEqual(0, money.Cents);
            Assert.AreEqual("$0.00", money.ToString());

            money.Dollars = 2;
            Assert.AreEqual(2, money.Dollars);
            Assert.AreEqual(0, money.Cents);
            Assert.AreEqual("$2.00", money.ToString());
            assert.Catch<ArgumentOutOfRangeException>(() => money.Dollars = -1);

            Assert.AreEqual(2, money.Dollars);
            Assert.AreEqual(0, money.Cents);
            Assert.AreEqual("$2.00", money.ToString());

            money.Cents = 59;
            Assert.AreEqual(2, money.Dollars);
            Assert.AreEqual(59, money.Cents);
            Assert.AreEqual("$2.59", money.ToString());

            money.Cents = 590;
            Assert.AreEqual(7, money.Dollars);
            Assert.AreEqual(90, money.Cents);
            Assert.AreEqual("$7.90", money.ToString());

            money.Cents = -456;
            // $7.00 - $4.56 = $2.44
            Assert.AreEqual(2, money.Dollars);
            Assert.AreEqual(44, money.Cents);
            Assert.AreEqual("$2.44", money.ToString());

            assert.Catch<ArgumentOutOfRangeException>(() => money.Cents = -245);
            Assert.AreEqual(2, money.Dollars);
            Assert.AreEqual(44, money.Cents);
            Assert.AreEqual("$2.44", money.ToString());
#if !DEBUG
});
#endif
        }
    }
}
