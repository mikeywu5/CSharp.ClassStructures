using System;
using NUnit.Framework;
using CSharp.Assignments.Tests.Library;
using System.Reflection;

namespace CSharp.Assignments.Classes.Employee1.Tests
{
    public class EmployeeTests
    {
        [Test]
        public void TestEmployeeClass()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            var employeeClass = new TypeAssert<Employee>();
            var firstName = employeeClass.Property<string>(
                "FirstName",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty
                ).AutoImplemented();
            var lastName = employeeClass.Property<string>(
                "LastName",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty
                ).AutoImplemented();
            var monthlySalaryField = employeeClass.Field<decimal>(
                "_monthlySalary",
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.GetField
                );
            var monthlySalary = employeeClass.Property<decimal>(
                "MonthlySalary",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty
                ).NotAutoImplemented();

            var annualSalary = employeeClass.Property<decimal>(
                "AnnualSalary",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty
                ).NotAutoImplemented();

            Employee employee = employeeClass.New("A", "B", 1234.56m);
            Assert.AreEqual(1234.56m, monthlySalaryField.GetValue(employee), "Private field must contain an intial value.");

#if !DEBUG
            });
#endif
        }

        [Test]
        public void TestEmployeeObject()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            var employeeClass = new TypeAssert<Employee>();
            dynamic employee = employeeClass.New("Mark", "Baker", 1234.56m);
            Assert.AreEqual("Mark", employee.FirstName, "Initial first name");
            Assert.AreEqual("Baker", employee.LastName, "Initial last name");
            Assert.AreEqual(1234.56m, employee.MonthlySalary, "Initial monthly Salary");
            Assert.AreEqual(14814.72m, employee.AnnualSalary, "Initial Annual Salary");
            employee.MonthlySalary = 123.45m;
            Assert.AreEqual(123.45m, employee.MonthlySalary, "Updated monthly Salary");
            Assert.AreEqual(1481.4m, employee.AnnualSalary, "Updated annual Salary");
            employee.MonthlySalary = -0.01m;
            Assert.AreEqual(123.45m, employee.MonthlySalary, "Updated monthly Salary with a negative value");
            Assert.AreEqual(1481.4m, employee.AnnualSalary, "Updated annual Salary (unchanged)");
#if !DEBUG
            });
#endif
        }

        [Test]
        public void TestEmployeeProgram()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            Action app = EmployeeTest.Main;

            string expected = app.Run();
            expected.Assert(
                ExpectTo.AssertContinuously | ExpectTo.Match,
                "Bob.+?Jones.+?34[,]?500", // Initial Employee 1
                "Susan.+?Baker.+?37[,]?809", // Initial Employee 2
                "Bob.+?Jones.+?37[,]?950", // Employee 1 after 10% pay raise
                "Susan.+?Baker.+?41[,]?589\\.9" // Employee 2 after 10% pay raise
                );
#if !DEBUG
            });
#endif
        }
    }
}
