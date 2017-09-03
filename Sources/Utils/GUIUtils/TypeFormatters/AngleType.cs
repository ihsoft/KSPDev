// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a numeric value that represents an <i>angle</i>. The
/// resulted message may have a unit specification.
/// </summary>
/// <remarks>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants.
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatFixed"/></example>
public sealed class AngleType {
  /// <summary>Suffix for the "angle" units (degrees).</summary>
  public const string unitName = "°";

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an angle type object.</summary>
  /// <param name="value">The numeric value in degrees.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatFixed"/></example>
  public AngleType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>A type object.</returns>
  public static implicit operator AngleType(double value) {
    return new AngleType(value);
  }

  /// <summary>Converts an angle type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(AngleType obj) {
    return obj.value;
  }

  /// <summary>Formats the value into a human friendly string with a unit specification.</summary>
  /// <remarks>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </remarks>
  /// <param name="value">The numeric value to format.</param>
  /// <param name="format">
  /// The specific float number format to use. If the format is not specified, then it's choosen
  /// basing on the value.
  /// </param>
  /// <returns>A formatted and localized string</returns>
  /// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/AngleType-Examples.cs" region="AngleTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, string format = null) {
    if (format != null) {
      return value.ToString(format) + unitName;
    }
    if (value < double.Epsilon) {
      return "0" + unitName;  // Zero is zero.
    }
    if (value < 1.0) {
      return value.ToString("0.0#") + unitName;
    }
    if (value < 10.0) {
      return value.ToString("0.#") + unitName;
    }
    return value.ToString("0") + unitName;
  }

  /// <summary>Returns a string formatted as a human friendly angle specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
