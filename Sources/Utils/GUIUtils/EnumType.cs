// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils {

/// <summary>Localized message formatting class for a enum value.</summary>
/// <remarks>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants. In
/// spite of the regular enum type, this wrapper returns an integer representation of the value when
/// casted to a string. This is vital for the Lingoona templates since they don't recognize the enum
/// values.
/// </para>
/// <para>
/// The conversion between the enum value and the wrapper can be done implicitly. The wrapper can be
/// used in the places where the enum value would normally be used, and vise versa.
/// </para>
/// </remarks>
/// <seealso cref="LocalizableMessage"/>
/// <seealso cref="DistanceType"/>
/// <seealso href="http://lingoona.com/cgi-bin/grammar#l=en&amp;oh=1">Lingoona Grammar help</seealso>
/// <example><code source="Examples/GUIUtils/EnumType-Examples.cs" region="EnumTypeDemo1"/></example>
public sealed class EnumType<T> where T : struct, IConvertible {
  readonly T value;

  /// <summary>Constructs a wrapper from the enum value.</summary>
  /// <param name="value">The enum value to wrap.</param>
  /// <example><code source="Examples/GUIUtils/EnumType-Examples.cs" region="EnumTypeDemo1"/></example>
  public EnumType(T value) {
    if (!typeof(T).IsEnum) {
      throw new ArgumentException("Type " + typeof(T) + " is not enum");
    }
    this.value = value;
  }

  /// <summary>Converts the enum value into a wrapper.</summary>
  /// <param name="value">The enum value to wrap.</param>
  /// <returns>A wrapped value.</returns>
  public static implicit operator EnumType<T>(T value) {
    return new EnumType<T>(value);
  }

  /// <summary>Converts the wrapper into a enum value.</summary>
  /// <param name="enumObj">The wrapper to convert.</param>
  /// <returns>A enum value.</returns>
  public static implicit operator T(EnumType<T> enumObj) {
    return enumObj.value;
  }

  /// <summary>Returns the int value converted into a string.</summary>
  /// <returns>A string representing the value.</returns>
  public override string ToString() {
    return Convert.ToInt32(value).ToString();
  }
}

}  // namespace
