using System;
using NUnit.Framework;
using CSharp.Assignments.Tests.Library;
using System.Reflection;

namespace CSharp.Assignments.Classes.Account1.Tests
{
    public class AccountTests
    {
        [Test]
        public void TestAccountClass()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif

            var accountClass = new TypeAssert<Account>();

            var balanceField = accountClass.Field<decimal>("_balance",
               BindingFlags.NonPublic |
               BindingFlags.Instance |
               BindingFlags.GetField |
               BindingFlags.SetField
               ).NonPublic();

            var nameProperty = accountClass.Property<string>("Name",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty);

            var balanceProperty = accountClass.Property<decimal>("Balance",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty);

            balanceProperty.NotAutoImplemented();

            var constructor = accountClass.Constructor(
                BindingFlags.Public | BindingFlags.Instance,
                new Param<string>("name"),
                new Param<decimal>("balance"));

            accountClass.Method("Deposit",
                BindingFlags.Public |
                BindingFlags.Instance,
                new Param<decimal>("depositAmount"));

            accountClass.Method("Withdraw",
                BindingFlags.Public |
                BindingFlags.Instance,
                new Param<decimal>("withdrawAmount")
                );

            // testing

            dynamic account;

            account = accountClass.New("Darth Vader", -1.1m);
            Assert.AreEqual(0m, balanceField.GetValue(account));
            Assert.AreEqual("Darth Vader", account.Name);
            Assert.AreEqual(0m, account.Balance);

            account = accountClass.New("Padme Amidala", 1.49m);
            Assert.AreEqual(1.49m, balanceField.GetValue(account));
            Assert.AreEqual("Padme Amidala", account.Name);
            Assert.AreEqual(1.49m, account.Balance);
#if !DEBUG
            });
#endif
        }

        [Test]
        public void TestAccountDepositWithdraw()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif

            var accountClass = new TypeAssert<Account>();
            dynamic account = accountClass.New("John Smith", 20m);
            Assert.AreEqual(20m, account.Balance, "Initial balance");
            account.Deposit(15.5m);
            Assert.AreEqual(35.5m, account.Balance, "20 + 15.5");
            account = accountClass.New("John Smith", 20m);
            account.Withdraw(15.5m);
            Assert.AreEqual(4.5m, account.Balance, "20 - 15.5");
            account = accountClass.New("John Smith", 20m);
            account.Deposit(-0.5m);
            Assert.AreEqual(20m, account.Balance, "cannot deposit negative amount.");
            account = accountClass.New("John Smith", 20m);

            Action tester = () =>
            {
                account.Withdraw(25m);
            };
            string output = tester.Run();
            Assert.AreEqual(20m, account.Balance, "cannot deposit negative amount.");
            output.Assert("Withdrawal amount exceeded account balance.");
#if !DEBUG
            });
#endif
        }


        [Test]
        public void TestAccountProgram()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif

            Action app = AccountTest.Main;
            var actual = app.Run(
                "Jane Green", 123.45m, // account1
                "John Blue", 54.32m,   // account2
                -0.04m, // account1 deposit
                76.4m, // account2 deposit
                121.44m, // account1 withdraw
                135m // account2 withdraw
                );

            actual.Assert(
                ExpectTo.AssertContinuously | ExpectTo.Match,
                @"Jane Green.*?123\.45", // amount1's initial balance
                @"John Blue.*?54\.32", // amount2's initial balance
                @"Jane Green.*?123\.45", // amount1's deposit
                @"John Blue.*?130\.72", // amount2's deposit
                @"Jane Green.*?2\.01", // amount1's withdraw
                @"Withdrawal amount exceeded account balance", // amount2's withdrawl error message
                @"John Blue.*130\.72" // amount2's withdraw
                );
#if !DEBUG
            });
#endif

        }
    }
}
