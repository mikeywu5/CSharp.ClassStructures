using System;
using NUnit.Framework;
using CSharp.Assignments.Tests.Library;
using System.Reflection;
namespace CSharp.Assignments.Classes.Structure.Tests
{
    public class StudentTests
    {
        [Test]
        public void Student()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Student>();
            assert.Property<string>(
                "FirstName",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty);
            assert.Property<string>(
                "LastName",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty);
            assert.Property<string>(
                "StudentNumber",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty);
            assert.Property<decimal>(
                "GPA",
                 BindingFlags.Instance |
                 BindingFlags.Public |
                 BindingFlags.GetProperty |
                 BindingFlags.SetProperty);
            assert.Property<string>(
                "Major",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty);
            assert.Constructor(
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<string>("firstName"),
                new Param<string>("lastName"),
                new Param<string>("studentNumber")
            );

            assert.Constructor(
                BindingFlags.Instance |
                BindingFlags.Public,
                new Param<string>("firstName"),
                new Param<string>("lastName"),
                new Param<string>("studentNumber"),
                new Param<decimal>("gpa"),
                new Param<string>("major")
                );
#if !DEBUG
        });
#endif
        }

        [Test]
        public void NewStudent()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Student>();
            dynamic student = assert.New("Test", "User", "1234567");
            Assert.AreEqual("Test", student.FirstName);
            Assert.AreEqual("User", student.LastName);
            Assert.AreEqual("1234567", student.StudentNumber);

            student = assert.New("User", "Test", "7654321", 3.5m, "MIS");
            Assert.AreEqual("User", student.FirstName);
            Assert.AreEqual("Test", student.LastName);
            Assert.AreEqual("7654321", student.StudentNumber);
            Assert.AreEqual(3.5m, student.GPA);
            Assert.AreEqual("MIS", student.Major);
#if !DEBUG
    });
#endif
        }

        [Test]
        public void GPA()
        {
#if !DEBUG
        Assert.Multiple(() => {
#endif
            var assert = new TypeAssert<Student>();
            assert.Catch<ArgumentOutOfRangeException>(() => assert.New("User", "Test", "7654321", 4.5m, "MIS"));

            dynamic student = assert.New("User", "Test", "7654321");
            assert.Catch<ArgumentOutOfRangeException>(
                () => student.GPA = -0.01m
            );
            assert.Catch<ArgumentOutOfRangeException>(
                () => student.GPA = 4.01m
            );
            student.GPA = 4.0m;
            Assert.AreEqual(4.0m, student.GPA);
            student.GPA = 0.0m;
            Assert.AreEqual(0.0m, student.GPA);
            student.GPA = 2.0m;
            Assert.AreEqual(2.0m, student.GPA);
            student.GPA = 3.0m;
            Assert.AreEqual(3.0m, student.GPA);
#if !DEBUG
});
#endif
        }
    }
}
