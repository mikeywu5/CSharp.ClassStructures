using System;
using NUnit.Framework;
using CSharp.Assignments.Tests.Library;
using System.Reflection;

namespace CSharp.Assignments.Classes.Invoice1.Tests
{
    public class InvoiceTests
    {
        [Test]
        public void TestInvoiceClass()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            var invoiceClass = new TypeAssert<Invoice>();

            var partNumber = invoiceClass.Property<string>("PartNumber",
                 BindingFlags.Public |
                 BindingFlags.Instance |
                 BindingFlags.GetProperty |
                 BindingFlags.SetProperty
             ).AutoImplemented();

            var partDescription = invoiceClass.Property<string>("PartDescription",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty
            ).AutoImplemented();

            var quantityField = invoiceClass.Field<int>("_quantity",
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            var pricePerItemField = invoiceClass.Field<decimal>("_pricePerItem",
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            var quantity = invoiceClass.Property<int>("Quantity",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.GetProperty |
                    BindingFlags.SetProperty
                ).NotAutoImplemented();

            var pricePerItem = invoiceClass.Property<decimal>("PricePerItem",
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty
            ).NotAutoImplemented();

            var constructor = invoiceClass.Constructor(
                BindingFlags.Public |
                BindingFlags.Instance,
                new Param<string>("partNumber"),
                new Param<string>("partDescription"),
                new Param<int>("quantity"),
                new Param<decimal>("price"));

            var getInvoiceAmount = invoiceClass.Method<decimal>(
                "GetInvoiceAmount",
                BindingFlags.Public |
                BindingFlags.Instance);

            // field testing
            Invoice invoice = invoiceClass.New("1234", "Hammer", 2, 14.95m);
            Assert.AreEqual(2, quantityField.GetValue(invoice), "Initial Quantity");
            Assert.AreEqual(14.95m, pricePerItemField.GetValue(invoice), "Initial price per item");
#if !DEBUG
            });
#endif
        }

        [Test]
        public void TestInvoiceObject()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            var invoiceClass = new TypeAssert<Invoice>();

            dynamic invoice = invoiceClass.New("1234", "Hammer", 2, 14.95m);
            Assert.AreEqual("1234", invoice.PartNumber, "Initial Part Number");
            Assert.AreEqual("Hammer", invoice.PartDescription, "Initial Part Description");
            Assert.AreEqual(2, invoice.Quantity, "Initial Quantity");
            Assert.AreEqual(14.95m, invoice.PricePerItem, "Initial Price Per Item");
            Assert.AreEqual(29.90m, invoice.GetInvoiceAmount(), "Initial GetInvoiceAmount()");

            invoice.PartNumber = "001234";
            invoice.PartDescription = "Yellow Hammer";
            invoice.Quantity = 3;
            invoice.PricePerItem = 19.49m;

            Assert.AreEqual("001234", invoice.PartNumber, "Updated Part Number");
            Assert.AreEqual("Yellow Hammer", invoice.PartDescription, "Updated Part Description");
            Assert.AreEqual(3, invoice.Quantity, "Updated Quantity");
            Assert.AreEqual(19.49m, invoice.PricePerItem, "Updated Price Per Item");
            Assert.AreEqual(58.47m, invoice.GetInvoiceAmount(), "Updated GetInvoiceAmount()");

            invoice.Quantity = -4;
            invoice.PricePerItem = -0.01m;
            Assert.AreEqual(3, invoice.Quantity, "Updated Quantity with a negative value");
            Assert.AreEqual(19.49m, invoice.PricePerItem, "Updated Price Per Item with a negative value");
            Assert.AreEqual(58.47m, invoice.GetInvoiceAmount(), "Updated GetInvoiceAmount() after alterations");
#if !DEBUG
            });
#endif
        }

        [Test]
        public void TestInvoiceProgram()
        {
#if !DEBUG
            Assert.Multiple(() => {
#endif
            Action app = InvoiceTest.Main;
            string actual = app.Run(
                "001234", // initial part number
                "Hammer", // initial part description
                451, // quantity
                1.79m, // price per item
                "ABCDEF", // updated part number
                "Golf Ball", // updated part description,
                98, // updated quantity,
                3.21m // updated price per item
                );
            actual.Assert(
                ExpectTo.AssertContinuously | ExpectTo.Contain,
                "001234", // initial part number
                "Hammer", // initial part descritpion
                "451", // initial quantity
                "1.79", // initial price per item
                "807.29", // initial invoice amount
                "ABCDEF", // updated part number
                "Golf Ball", // updated description
                "98", // updated quantity
                "3.21", // updated price per item
                "314.58" // updated invoice amount
                );

            actual = app.Run(
                "001234", // initial part number
                "Hammer", // initial part description
                451, // quantity
                1.79m, // price per item
                "001234", // updated part number
                "Hammer", // updated part description,
                -1, // updated quantity,
                -0.5m // updated price per item
                );
            actual.Assert(
                ExpectTo.AssertContinuously | ExpectTo.Contain,
                "001234", // initial part number
                "Hammer", // initial part descritpion
                "451", // initial quantity
                "1.79", // initial price per item
                "807.29", // initial invoice amount
                "001234", // updated part number
                "Hammer", // updated description
                "451", // updated quantity with a negative value
                "1.79", // updated price per item with a negative value
                "807.29" // updated invoice amount overall
                );
#if !DEBUG
            });
#endif
        }
    }
}
