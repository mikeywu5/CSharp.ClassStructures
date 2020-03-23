using System;
using NUnit.Framework;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CSharp.Assignments.Tests.Library
{
    //[DebuggerNonUserCode]
    public class TypeAssert
    {
        /// <summary>
        /// Gets the type that is being asserted.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="type">asserted type.</param>
        public TypeAssert(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Create a new instance of the class using the given parameters. If the constructor is missing, assert fail.
        /// </summary>
        /// <returns>A new instance.</returns>
        /// <param name="parameters">Parameters.</param>
        public dynamic New(params object[] parameters)
        {
            try
            {
                return Activator.CreateInstance(Type, parameters == null ? new object[] { null } : parameters);
            }
            catch (MissingMethodException)
            {
                string parameterTypes;
                if (parameters == null || parameters.Length == 0)
                {
                    parameterTypes = "parameterless arguments";
                }
                else
                {
                    parameterTypes = $"parameter types ({string.Join(", ", parameters.Select(p => p == null ? "null" : p.GetType().Name))})";
                }
                Assert.Fail($"Failed to create a new instance in {Type.Name} because the constructor that accepts {parameterTypes} in the {Type.Name} Type is missing or inaccessible.");
                return null;
            }
        }

        /// <summary>
        /// Asserts whether it is an interface.
        /// </summary>
        public void Interface()
        {
            if (!Type.IsInterface)
            {
                Assert.Fail($"The {Type.Name} Type must be an interface.");
            }
        }

        /// <summary>
        /// Asserts whether it is an enum.
        /// </summary>
        public void Enum()
        {
            if (!Type.IsEnum)
            {
                Assert.Fail($"The {Type.Name} Type must be an enum.");
            }
        }

        /// <summary>
        /// Asserts whether it is a class.
        /// </summary>
        public void Class()
        {
            if (!Type.IsClass)
            {
                Assert.Fail($"The {Type.Name} Type must be a class.");
            }
        }

        /// <summary>
        /// Asserts whether it is an <see langword="abstract"/>
        /// </summary>
        public void Abstract()
        {
            if (!Type.IsAbstract)
            {
                Assert.Fail($"The {Type.Name} Type must be an abstract class.");
            }
        }

        /// <summary>
        /// Asserts whether it is not <see langword="abstract"/>
        /// </summary>
        public void NonAbstract()
        {
            if (Type.IsAbstract)
            {
                Assert.Fail($"The {Type.Name} Type must not be an abstract class.");
            }
        }

        /// <summary>
        /// Asserts whether it is <see langword="sealed"/>
        /// </summary>
        public void Sealed()
        {
            if (!Type.IsSealed)
            {
                Assert.Fail($"The {Type.Name} Type must be a sealed class.");
            }
        }

        /// <summary>
        /// Asserts whether it is not <see langword="sealed"/>
        /// </summary>
        public void NonSealed()
        {
            if (Type.IsSealed)
            {
                Assert.Fail($"The {Type.Name} Type must be not a sealed class.");
            }
        }

        /// <summary>
        /// Asserts whether the class extends another class.
        /// </summary>
        /// <param name="c">A class.</param>
        public void Extends<ExtendedType>()
        {
            Type type = typeof(ExtendedType);
            Assert.AreEqual(type, Type.BaseType, $"The {Type.Name} Type must have a base type of {type.Name}, but it has a base type of {Type.BaseType.Name}.");
        }

        /// <summary>
        /// Asserts whether the class implements the correct interfaces.
        /// </summary>
        /// <param name="interfaces">Interfaces.</param>
        public void Implements<Interface>()
        {
            var interfce = typeof(Interface);
            var expected = Type.GetInterfaces();
            CollectionAssert.Contains(expected, interfce, $"The {Type.Name} Type must implement the interface {interfce.Name}.");
        }

        /// <summary>
        /// Asserts whether the class implements the correct interfaces.
        /// </summary>
        /// <param name="interfaces">Interfaces.</param>
        public void NotImplements<Interface>()
        {
            var interfce = typeof(Interface);
            var expected = Type.GetInterfaces();
            CollectionAssert.DoesNotContain(expected, interfce, $"The {Type.Name} Type MUST NOT implement the interface {interfce.Name}.");
        }

        /// <summary>
        /// Asserts whether the class has the field
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        public FieldInfo Field<FieldType>(string name, BindingFlags bindings)
        {
            Type fieldType = typeof(FieldType);
            FieldInfo fieldInfo = Type.GetFields(bindings).FirstOrDefault(f => f.Name == name);
            if (fieldInfo == null)
            {
                Assert.Fail($"The {Type.Name} Type must have a(n) {bindings} field with a name of \"{name}\" and a type of {fieldType.Name}.");
            }
            return fieldInfo;
        }

        public FieldInfo NonField<FieldType>(string name, BindingFlags bindings)
        {
            Type fieldType = typeof(FieldType);
            FieldInfo fieldInfo = Type.GetFields(bindings).FirstOrDefault(f => f.Name == name);
            if (fieldInfo != null)
            {
                Assert.Fail($"The {Type.Name} Type must NOT have a(n) {bindings} field with a name of \"{name}\" and a type of {fieldType.Name}.");
            }
            return fieldInfo;
        }

        /// <summary>
        /// Asserts whether the class has the property
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        public PropertyInfo Property<ProperyType>(string name, BindingFlags bindings)
        {
            Type propertyType = typeof(ProperyType);
            PropertyInfo propertyInfo = Type.GetProperty(name, bindings);
            MethodInfo get = propertyInfo?.GetGetMethod();
            MethodInfo set = propertyInfo?.GetSetMethod();
            bool hasGet = (bindings & BindingFlags.GetProperty) == BindingFlags.GetProperty;
            bool hasSet = (bindings & BindingFlags.SetProperty) == BindingFlags.SetProperty;
            if (!(propertyInfo != null &&
                          (get == null ? !hasGet
                           : hasGet && get.ReturnType == propertyType) &&
                          (set == null ? !hasSet
                           : hasSet && set.GetParameters().FirstOrDefault()?.ParameterType == propertyType)))
            {
                Assert.Fail($"The {Type.Name} Type must have a(n) {bindings} property with a name of \"{name}\" and a type of {propertyType.Name}.");
            }
            return propertyInfo;
        }

        /// <summary>
        /// Asserts whether the class has that method.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public MethodInfo Method<ReturnType>(string name, BindingFlags bindings, params Param[] parameterTypes)
        {
            Type returnType = typeof(ReturnType);
            var ptypes = GetTypes(parameterTypes);
            MethodInfo methodInfo = Type.GetMethod(name, bindings, null, ptypes, null);
            string validation = methodInfo != null ? Param.Validate(parameterTypes, methodInfo.GetParameters()) : null;
            if (!string.IsNullOrEmpty(validation) || methodInfo == null || methodInfo.ReturnParameter.ParameterType != returnType)
            {
                Assert.Fail($"The {Type.Name} Type must have a {bindings} method with a name of \"{name}\", a return type of {returnType.Name}, and parameters of ({string.Join<Param>(", ", parameterTypes)}).{validation}");
            }
            return methodInfo;
        }

        /// <summary>
        /// Asserts whether the class has that method.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public MethodInfo Method(string name, BindingFlags bindings, params Param[] parameterTypes)
        {
            Type returnType = typeof(void);
            MethodInfo methodInfo = Type.GetMethod(name, bindings, null, GetTypes(parameterTypes), null);
            string validation = methodInfo != null ? Param.Validate(parameterTypes, methodInfo.GetParameters()) : null;
            if (!string.IsNullOrEmpty(validation) || methodInfo == null || methodInfo.ReturnParameter.ParameterType != returnType)
            {
                Assert.Fail($"The {Type.Name} Type must have a {bindings} method with a name of \"{name}\", a return type of {returnType.Name}, and parameters of ({string.Join<Param>(", ", parameterTypes)}).{validation}");
            }
            return methodInfo;
        }


        /// <summary>
        /// Asserts whether the class has the constructor.
        /// </summary>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public ConstructorInfo Constructor(BindingFlags bindings, params Param[] parameterTypes)
        {
            ConstructorInfo constructor = Type.GetConstructor(bindings, null, GetTypes(parameterTypes), null);
            string validation = constructor == null ? null : Param.Validate(parameterTypes, constructor.GetParameters());
            if (!string.IsNullOrEmpty(validation) || constructor == null)
            {
                Assert.Fail($"The {Type.Name} Type must have a(n) {bindings} constructor with parameters of ({string.Join<Param>(", ", parameterTypes)}).{validation}");
            }
            return constructor;
        }

        /// <summary>
        /// Asserts whether the class has the property
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        public PropertyInfo NonProperty<ProperyType>(string name, BindingFlags bindings)
        {
            Type propertyType = typeof(ProperyType);
            PropertyInfo propertyInfo = Type.GetProperty(name, bindings);
            MethodInfo get = propertyInfo?.GetGetMethod();
            MethodInfo set = propertyInfo?.GetSetMethod();
            if (propertyInfo != null &&
                          (get == null ? (bindings & BindingFlags.GetProperty) != BindingFlags.GetProperty
                           : (bindings & BindingFlags.GetProperty) == BindingFlags.GetProperty && get.ReturnType == propertyType) &&
                          (set == null ? (bindings & BindingFlags.SetProperty) != BindingFlags.SetProperty
                           : (bindings & BindingFlags.GetProperty) == BindingFlags.GetProperty && set.GetParameters().FirstOrDefault()?.ParameterType == propertyType))
            {
                Assert.Fail($"The {Type.Name} Type must NOT have a(n) {bindings} property with a name of \"{name}\" and a type of {propertyType.Name}.");
            }
            return propertyInfo;
        }

        /// <summary>
        /// Asserts whether the class has that method.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public MethodInfo NonMethod<ReturnType>(string name, BindingFlags bindings, params Param[] parameterTypes)
        {
            Type returnType = typeof(ReturnType);
            MethodInfo methodInfo = Type.GetMethod(name, bindings, null, GetTypes(parameterTypes), null);
            if (methodInfo != null && methodInfo.ReturnParameter.ParameterType == returnType)
            {
                Assert.Fail($"The {Type.Name} Type must NOT have a {bindings} method with a name of \"{name}\", a return type of {returnType.Name}, and parameters of ({string.Join<Param>(", ", parameterTypes)}).");
            }
            return methodInfo;
        }
        /// <summary>
        /// Asserts whether the class has that method.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public MethodInfo NonMethod(string name, BindingFlags bindings, params Param[] parameterTypes)
        {
            Type returnType = typeof(void);
            MethodInfo methodInfo = Type.GetMethod(name, bindings, null, GetTypes(parameterTypes), null);
            if (methodInfo != null && methodInfo.ReturnParameter.ParameterType == returnType)
            {
                Assert.Fail($"The {Type.Name} Type must NOT have a {bindings} method with a name of \"{name}\", a return type of {returnType.Name}, and parameters of ({string.Join<Param>(", ", parameterTypes)}).");
            }
            return methodInfo;
        }

        /// <summary>
        /// Asserts whether the class has the constructor.
        /// </summary>
        /// <param name="bindings">Bindings.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        public ConstructorInfo NonConstructor(BindingFlags bindings, params Param[] parameterTypes)
        {
            ConstructorInfo constructor = Type.GetConstructor(bindings, null, GetTypes(parameterTypes), null);
            if (constructor != null)
            {
                Assert.Fail($"The {Type.Name} Type must NOT have a(n) {bindings} constructor with parameters of ({string.Join<Param>(", ", parameterTypes)}).");
            }
            return constructor;
        }

        /// <summary>
        /// Asserts a count to the number of constructors
        /// </summary>
        /// <param name="count">Count.</param>
        /// <param name="bindings">Bindings.</param>
        public void ConstructorCount(int count, BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
        {
            ConstructorInfo[] constructors = Type.GetConstructors(bindings);
            Assert.AreEqual(count, constructors.Length, $"The {Type.Name} Type must have exactly {count} {bindings} constructor(s).");
        }

        /// <summary>
        /// Convert from an array of Param types to an array of Types.
        /// </summary>
        /// <returns>The types.</returns>
        /// <param name="parameterTypes">Parameter types.</param>
        private Type[] GetTypes(params Param[] parameterTypes)
        {
            Type[] types = new Type[parameterTypes.Length];
            for (int i = 0; i < types.Length; i++)
            {
                var ptype = parameterTypes[i];
                if (ptype.Out || ptype.Ref)
                {
                    types[i] = ptype.Type.MakeByRefType();
                }
                else
                {
                    types[i] = ptype;
                }
            }
            return types;
        }

        /// <summary>
        /// Asserting when an exception is caught
        /// </summary>
        /// <returns>exception.</returns>
        /// <param name="test">Test code.</param>
        /// <typeparam name="TException">The 1st type parameter.</typeparam>
        public static void Catch<TException>(Action test)
            where TException : Exception
        {
            try
            {
                test();
                Assert.Fail($"Expect to throw an exception of {typeof(TException)}, but there is no exception thrown at all.");
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    Assert.IsInstanceOf<TException>(e.InnerException, $"Incorrect Exception is thrown. {(string.IsNullOrEmpty(e.InnerException.Message) ? "" : Environment.NewLine)}{e.InnerException.Message}");
                }
                else
                {
                    throw;
                }
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Assert.IsInstanceOf<TException>(exception, $"Incorrect Exception is thrown. {(string.IsNullOrEmpty(exception.Message) ? "" : Environment.NewLine)}{exception.Message}");
            }
        }
    }
    /// <summary>
    /// Typical assertions of a class structure
    /// </summary>
    [DebuggerNonUserCode]
    public class TypeAssert<T> : TypeAssert
    {
        /// <summary>
        /// Initializes a new instance of the class with a generic type that will be asserted.
        /// </summary>
        public TypeAssert()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Create a new instance of the class using the given parameters. If the constructor is missing, assert fail.
        /// </summary>
        /// <returns>A new object</returns>
        /// <param name="parameters">Parameters.</param>
        public new T New(params object[] parameters)
        {
            return base.New(parameters);
        }

        /// <summary>
        /// Asserting an exception being caught
        /// </summary>
        /// <returns>exception.</returns>
        /// <param name="test">Test code.</param>
        /// <typeparam name="TException">The 1st type parameter.</typeparam>
        public new void Catch<TException>(Action test)
            where TException : Exception
        {
            TypeAssert.Catch<TException>(test);
        }
    }

    /// <summary>
    /// Type-specific Param for easy-to-read assertions to arguments and variables
    /// </summary>
    public class Param
    {
        public bool HasDefault { get; private set; }
        private object _default;
        /// <summary>
        /// Gets the type associated to the parameter.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }
        /// <summary>
        /// Gets the name associated to the parameter.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Initializes a new instance of the class with a given type and name.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        public Param(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public bool Ref { get; set; }
        public bool Out { get; set; }
        public bool Params { get; set; }
        public object Default
        {
            get
            {
                return _default;
            }
            set
            {
                HasDefault = true;
                _default = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents to look like the parameter codes.
        /// </summary>
        /// <returns>Parameter codes.</returns>
        public override string ToString()
        {
            string prefix;
            string suffix;
            if (Ref)
            {
                prefix = "ref ";
            }
            else if (Out)
            {
                prefix = "out ";
            }
            else if (Params)
            {
                prefix = "params ";
            }
            else
            {
                prefix = "";
            }
            if (HasDefault)
            {
                suffix = $" = {Utils.ToCode(Default)}";
            }
            else
            {
                suffix = "";
            }
            return $"{prefix}{Type.Name} {Name}{suffix}";
        }
        /// <summary>
        /// Convert to Type object implicitly
        /// </summary>
        /// <returns>implicit Type.</returns>
        /// <param name="p">parameter object</param>
        public static implicit operator Type(Param p)
        {
            return p.Type;
        }

        /// <summary>
        /// Match and assert the expected parameters against the actual parameters
        /// </summary>
        /// <param name="expected">Expected.</param>
        /// <param name="actual">Actual.</param>
        /// <returns>A Validation message if a parameter is invalid</returns>
        public static string Validate(Param[] expected, ParameterInfo[] actual)
        {
            if (expected.Length != actual.Length)
            {
                return $" The expected number ({expected.Length}) of parameters does not match with the actual number ({actual.Length}) of parameters.";
            }

            var sb = new StringBuilder();
            for (int i = 0; i < expected.Length; i++)
            {
                var e = expected[i];
                var a = actual[i];
                // Assert.AreEqual(e.Name, a.Name, $"The name does not match in Parameter {i}.");
                if (e.Ref && (!a.IsIn || !a.IsOut))
                {
                    sb.Append($" The parameter \"{e.Name}\" requires a ref keyword.");
                }
                if (e.Out && (a.IsIn || !a.IsOut))
                {
                    sb.Append($" The parameter \"{e.Name}\" requires an out keyword.");
                }
                if (e.Params && !a.GetCustomAttributes(typeof(ParamArrayAttribute), false).Any())
                {
                    sb.Append($" The parameter \"{e.Name}\" requires a params keyword.");
                }
                if (e.HasDefault && !Equals(a.DefaultValue, e.Default))
                {
                    sb.Append($" The parameter \"{e.Name}\" requires a default value of {Utils.ToCode(e.Default)}.");
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Generic Param for easy-to-read assertions to arguments and variables
    /// </summary>
    [DebuggerNonUserCode]
    public class Param<T> : Param
    {
        public new T Default
        {
            get { return (T)base.Default; }
            set { base.Default = value; }
        }

        public Param(string name)
            : base(typeof(T), name)
        {
        }
    }

    /// <summary>
    /// Member Info extensions for additional structural assertions.
    /// </summary>
    [DebuggerNonUserCode]
    public static class TypeAssertExtensions
    {
        /// <summary>
        /// Assert field if the given field is not public
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="field">Field.</param>
        public static FieldInfo Public(this FieldInfo field)
        {
            if (!field.IsPublic)
            {
                Assert.Fail($"The field {field} must be public in the {field.DeclaringType.Name} class.");
            }
            return field;
        }

        /// <summary>
        /// Assert field if the given field is public
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="field">Field.</param>
        public static FieldInfo NonPublic(this FieldInfo field)
        {
            if (field.IsPublic)
            {
                Assert.Fail($"The field {field} must NOT be public in the {field.DeclaringType.Name} class.");
            }
            return field;
        }

        /// <summary>
        /// Assert field if the given field is not protected
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="field">Field.</param>
        public static FieldInfo Protected(this FieldInfo field)
        {
            if (!field.IsFamily)
            {
                Assert.Fail($"The field {field} must be protected in the {field.DeclaringType.Name} class.");
            }
            return field;
        }

        /// <summary>
        /// Assert field if the given field is not private
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="field">Field.</param>
        public static FieldInfo Private(this FieldInfo field)
        {
            if (!field.IsPrivate)
            {
                Assert.Fail($"The field {field} must be private in the {field.DeclaringType.Name} class.");
            }
            return field;
        }

        /// <summary>
        /// Assert field if the given field is not read-only
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="field">Field.</param>
        public static FieldInfo ReadOnly(this FieldInfo field)
        {
            if (!field.IsInitOnly)
            {
                Assert.Fail($"The field {field} must be readonly in the {field.DeclaringType.Name} class.");
            }
            return field;
        }

        /// <summary>
        /// Assert method if the given method is not public
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Public(this MethodInfo method)
        {
            if (!method.IsPublic)
            {
                Assert.Fail($"The method {method} must be public in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assrt fail if the given method is public
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo NonPublic(this MethodInfo method)
        {
            if (method.IsPublic)
            {
                Assert.Fail($"The method {method} must be non-public in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is not private
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Private(this MethodInfo method)
        {
            if (!method.IsPrivate)
            {
                Assert.Fail($"The method {method} must be private in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is not protected
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Protected(this MethodInfo method)
        {
            if (!method.IsFamily)
            {
                Assert.Fail($"The method {method} must be private in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assrt fail if the given method is not virtual
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Virtual(this MethodInfo method)
        {
            if (!method.IsVirtual)
            {
                Assert.Fail($"The method {method} must be virtual in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assdrt fail if the given method is not overridden
        /// </summary>
        /// <returns>The method.</returns>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Override(this MethodInfo method)
        {
            Assert.AreNotEqual(method, method.GetBaseDefinition(), $"The keyword \"override\" may be missing when the method {method} is overridden in the {method.DeclaringType.Name} class.");
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is not abstract
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo Abstract(this MethodInfo method)
        {
            if (!method.IsAbstract)
            {
                Assert.Fail($"The method {method} must be abstract in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is virtual
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo NonVirtual(this MethodInfo method)
        {
            if (method.IsVirtual)
            {
                Assert.Fail($"The method {method} must NOT be virtual in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is overridden
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo NonOverride(this MethodInfo method)
        {
            Assert.AreEqual(method, method.GetBaseDefinition(), $"The method {method} in the {method.DeclaringType.Name} class must NOT be overriden.");
            return method;
        }

        /// <summary>
        /// Assert fail if the given method is abstract
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="method">Method.</param>
        public static MethodInfo NonAbstract(this MethodInfo method)
        {
            if (method.IsAbstract)
            {
                Assert.Fail($"The method {method} must NOT be abstract in the {method.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Validate the getter method and the setter method, if exist, of a property against the check.
        /// </summary>
        /// <returns><see langword="true"/> if the getter and the setter have been validated.</returns>
        /// <param name="property">Property.</param>
        /// <param name="check">Check.</param>
        private static bool What(this PropertyInfo property, Func<MethodInfo, bool> check)
        {
            MethodInfo get = property.GetGetMethod();
            MethodInfo set = property.GetSetMethod();
            bool b = (get == null || check(get)) && (set == null || check(set));
            return b;
        }

        /// <summary>
        /// Assert fail if the given property is not public
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Public(this PropertyInfo property)
        {
            if (!property.What(m => m.IsPublic))
            {
                Assert.Fail($"The property {property} must be public in the {property.DeclaringType.Name} class.");
            }
            return property;
        }
        /// <summary>
        /// Assert fail if the given property is public
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo NonPublic(this PropertyInfo property)
        {
            if (property.What(m => m.IsPublic))
            {
                Assert.Fail($"The property {property} must be non-public in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is not protected
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Protected(this PropertyInfo property)
        {
            if (!property.What(m => m.IsFamily))
            {
                Assert.Fail($"The property {property} must be private in the {property.DeclaringType.Name} class.");
            }
            return property;
        }
        /// <summary>
        /// Assert fail if the given property is not private
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Private(this PropertyInfo property)
        {
            if (!property.What(m => m.IsPrivate))
            {
                Assert.Fail($"The property {property} must be private in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is not virtual
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Virtual(this PropertyInfo property)
        {
            if (!property.What(m => m.IsVirtual))
            {
                Assert.Fail($"The property {property} must be virtual in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is not overridden.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Override(this PropertyInfo property)
        {
            if (!property.What(m => m != m.GetBaseDefinition()))
            {
                Assert.Fail($"The keyword \"override\" may be missing when the property {property} is overridden in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is not abstract
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo Abstract(this PropertyInfo property)
        {
            if (!property.What(m => m.IsAbstract))
            {
                Assert.Fail($"The property {property} must be abstract in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is virtual.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo NonVirtual(this PropertyInfo property)
        {
            if (property.What(m => m.IsVirtual))
            {
                Assert.Fail($"The property {property} must NOT be virtual in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is overridden.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo NonOverride(this PropertyInfo property)
        {
            if (property.What(m => m != m.GetBaseDefinition()))
            {
                Assert.Fail($"The property {property} in the {property.DeclaringType.Name} class must NOT be overriden.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is <see langword="abstract"/>.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo NonAbstract(this PropertyInfo property)
        {
            if (property.What(m => m.IsAbstract))
            {
                Assert.Fail($"The property {property} must NOT be abstract in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is not auto-implemented.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>

        public static PropertyInfo AutoImplemented(this PropertyInfo property)
        {
            if (!property.What(m => m.GetCustomAttributes(
                           typeof(CompilerGeneratedAttribute),
                           true
                       ).Any()))
            {
                Assert.Fail($"The property {property} must be auto-implemented in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the given property is auto-implemented.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="property">Property.</param>
        public static PropertyInfo NotAutoImplemented(this PropertyInfo property)
        {
            if (property.What(m => m.GetCustomAttributes(
                           typeof(CompilerGeneratedAttribute),
                           true
                       ).Any()))
            {
                Assert.Fail($"The property {property} must NOT be auto-implemented in the {property.DeclaringType.Name} class.");
            }
            return property;
        }

        /// <summary>
        /// Assert fail if the field is not declared in the given class of declaring type.
        /// </summary>
        /// <returns>The field for chain of commands.</returns>
        /// <param name="member">field.</param>
        /// <typeparam name="DeclaringType">The 1st type parameter.</typeparam>
        public static FieldInfo DeclaredIn<DeclaringType>(this FieldInfo member)
        {
            Type declaringType = typeof(DeclaringType);
            Assert.AreEqual(declaringType, member.DeclaringType, $"The field {member} must be declared in the {declaringType.Name} class.");
            return member;
        }

        /// <summary>
        /// Assert fail if the property is not declared in the given class of declaring type.
        /// </summary>
        /// <returns>The property for chain of commands.</returns>
        /// <param name="member">property.</param>
        /// <typeparam name="DeclaringType">The 1st type parameter.</typeparam>
        public static PropertyInfo DeclaredIn<DeclaringType>(this PropertyInfo member)
        {
            Type declaringType = typeof(DeclaringType);
            Assert.AreEqual(declaringType, member.DeclaringType, $"The property {member} must be declared in the {declaringType.Name} class.");
            return member;
        }

        /// <summary>
        /// Assert fail if the method is not declared in the given class of declaring type.
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="member">method.</param>
        /// <typeparam name="DeclaringType">The 1st type parameter.</typeparam>
        public static MethodInfo DeclaredIn<DeclaringType>(this MethodInfo member)
        {
            Type declaringType = typeof(DeclaringType);
            Assert.AreEqual(declaringType, member.DeclaringType, $"The method {member} must be declared in the {declaringType.Name} class.");
            return member;
        }

        /// <summary>
        /// Get the property getter method. Assert fail if it's not found.
        /// </summary>
        /// <returns>The getter method for chain of commands.</returns>
        /// <param name="property">Property.</param>
        /// <param name="includesNonPublic">If set to <c>true</c> includes non-public getter.</param>
        public static MethodInfo Getter(this PropertyInfo property, bool includesNonPublic = false)
        {
            MethodInfo method = property.GetGetMethod();
            if (method != null) return method;

            if (!includesNonPublic)
            {
                Assert.Fail($"The public getter of the property {property} cannot be found in the {property.DeclaringType.Name} class.");
            }
            method = property.GetGetMethod(true);
            if (method == null)
            {
                Assert.Fail($"The public or non-public getter of the property {property} cannot be found in the {property.DeclaringType.Name} class.");
            }
            return method;
        }

        /// <summary>
        /// Get the property setter method. Assert fail if it's not found.
        /// </summary>
        /// <returns>The method for chain of commands.</returns>
        /// <param name="property">Property.</param>
        /// <param name="includesNonPublic">If set to <c>true</c> includes non-public setter.</param>
        public static MethodInfo Setter(this PropertyInfo property, bool includesNonPublic = false)
        {
            MethodInfo method = property.GetSetMethod();
            if (method != null) return method;
            if (!includesNonPublic)
            {
                Assert.Fail($"The public setter of the property {property} cannot be found in the {property.DeclaringType.Name} class.");
            }
            method = property.GetSetMethod(true);
            if (method == null)
            {
                Assert.Fail($"The public or non-public setter of the property {property} cannot be found in the {property.DeclaringType.Name} class.");
            }
            return method;
        }
    }
}