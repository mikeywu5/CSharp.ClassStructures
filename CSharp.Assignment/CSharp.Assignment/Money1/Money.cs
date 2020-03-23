using System;
namespace CSharp.Assignments.Classes.Structure
{
    /// <summary>
    /// Create a Money class that has data members dollars and cents.
    ///   Include constructors that enable the Money class to be instantiated with a single value representing the full dollar/cent amount in decimal.
    ///   Include a constructor that enables you to create an instance of the class by sending two separate integer values representing the dollar and cent amounts.
    ///   Include another constructor that lets you initialize values from an existing Money object.
    ///   Include IncrementMoney and DecrementMoney instance methods that take the Money instance to increment or decrement the money. 
    ///   Include a public property named Dollars that gets and sets the value of the dollars. Dollars must not be negative; otherwise, throw an ArgumentOutOfRangeException.
    ///   Include a public property named Cents that gets and sets the value of the cents. When cents are negative, subtract them from the Dollars and then make the cents positive again.
    ///   Override the ToString( ) method to return the monetary amount formatted with currency symbols.
    /// </summary>
    public class Money
    {
    }
}
