// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.ConfigUtils {

/// <summary>A base class for a proto of a single value.</summary>
/// <remarks>All descendands of this class must implement a default constructor.</remarks>
/// <example>See real overrides in <see cref="PrimitiveTypesProto"/> and
/// <see cref="KspTypesProto"/>.
/// <para>Here is how you could implement your own proto to persist string array as a string.</para>
/// <code>
/// class StringArrayProto : AbstractOrdinaryValueTypeProto {
///   public override bool CanHandle(Type type) {
///     return typeof(string[]) == type;
///   }
///   public override string SerializeToString(object value) {
///     return string.Join(",", (value as string[]));
///   }
///   public override object ParseFromString(string value, Type type) {
///     // Due to check in CanHandle we know the type is string[].
///     return value.Split(',');
///   }
/// }
/// </code>
/// </example>
/// <seealso cref="ConfigAccessor"/>
/// <seealso cref="AbstractPersistentFieldAttribute"/>
public abstract class AbstractOrdinaryValueTypeProto {
  private AbstractOrdinaryValueTypeProto(params object[] args) {
    // Disallow parameterized constructors. This class will only be created via reflection.
  }

  /// <summary>Default constructor must be the only constructor of the proto.</summary>
  protected AbstractOrdinaryValueTypeProto() {}

  /// <summary>Tells if proto can handle the specified type.</summary>
  /// <param name="type">A type in question.</param>
  /// <returns><c>true</c> if proto can (de)serialize values of the type.</returns>
  public abstract bool CanHandle(Type type);

  /// <summary>Serializes value into a string.</summary>
  /// <remarks>In general avoid using <c>ToString()</c> methods to produce the serialized value.
  /// Such methods are not designed to be unambiguous.</remarks>
  /// <param name="value">A value to serialize.</param>
  /// <returns>A string representation of the value. It doesn't need to be human readable.</returns>
  public abstract string SerializeToString(object value);

  /// <summary>Makes a value from the string representation.</summary>
  /// <param name="value">A string produced by <see cref="SerializeToString"/>.</param>
  /// <param name="type">A type to convert the value into.</param>
  /// <returns>A new and initialized instance of the requested type.</returns>
  /// <exception cref="ArgumentException">If value cannot be parsed.</exception>
  public abstract object ParseFromString(string value, Type type);
}

}  // namespace
