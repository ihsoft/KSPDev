// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>distance</i>. The
/// resulted message may have a unit specification.
/// </summary>
/// <remarks>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants.
/// </para>
/// <para>
/// The class uses the unit name localizations from the stock module <c>ModuleEnviroSensor</c>. In
/// case of this module is deprecated or the tags are changed, the default English values will be
/// used for the unit names.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
public sealed class DistanceType {
  /// <summary>Localized suffix for the "meter" units. Scale x1.</summary>
  public static readonly Message meter = new Message(
      "#autoLOC_7001411", defaultTemplate: "m",
      description: "Meter unit for a distance value");

  /// <summary>Localized suffix for the "kilometer" untis. Scale x1000</summary>
  public static readonly Message kilometer = new Message(
      "#autoLOC_7001405", defaultTemplate: " km",
      description: "Kilometer unit for a distance value");

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an object from a numeric value.</summary>
  /// <param name="value">The numeric value in meters.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
  public DistanceType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator DistanceType(double value) {
    return new DistanceType(value);
  }

  /// <summary>Converts a type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(DistanceType obj) {
    return obj.value;
  }

  /// <summary>Formats the value into a human friendly string with a unit specification.</summary>
  /// <remarks>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </remarks>
  /// <param name="value">The unscaled numeric value to format.</param>
  /// <param name="scale">
  /// The fixed scale to apply to the value before formatting. The formatting method can uderstand
  /// only a few scales:
  /// <list type="bullet">
  /// <item>Meters: scale=<c>1.0</c>. <i>It's a base distance unit in the game.</i></item>
  /// <item>Kilometers: scale=<c>1.0e+3</c>.</item>
  /// </list>
  /// <para>
  /// The unknown scales will be rounded <i>down</i> to the closest known scale. If this parameter
  /// is omitted, then the best scale for the value will be choosen automatically.
  /// </para>
  /// </param>
  /// <param name="format">
  /// The specific numeric number format to use. If this parameter is specified, then the method
  /// doesn't try to guess the right scale. Instead, it uses either the provided
  /// <paramref name="scale"/>, or <c>1.0</c> if nothing is provided. If the format is not
  /// specified, then it's choosen basing on the scale.
  /// </param>
  /// <returns>A formatted and localized string</returns>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/DistanceType-Examples.cs" region="DistanceTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, double? scale = null, string format = null) {
    // Detect the scale, and scale the value.
    string units;
    double scaledValue;
    if (format != null && !scale.HasValue) {
      scale = 1.0;  // No scale detection.
    }
    if (!scale.HasValue) {
      // Auto detect the best scale.
      if (value > 1000) {
        scale = 1000;
      } else {
        scale = 1.0;
      }
    }
    if (scale <= 1.0) {
      scaledValue = value;
      units = meter;
    } else {
      scaledValue = value / 1000;
      units = kilometer;
    }
    if (format != null) {
      return scaledValue.ToString(format) + units;
    }
    return CompactNumberType.Format(scaledValue) + units;
  }

  /// <summary>Returns a string formatted as a human friendly distance specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
