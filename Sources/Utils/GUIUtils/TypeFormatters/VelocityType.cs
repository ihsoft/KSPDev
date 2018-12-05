// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils.TypeFormatters {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>velocity</i>. The
/// resulted message may have a unit specification.
/// </summary>
/// <remarks>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants.
/// </para>
/// <para>
/// The class uses the unit name localizations from the stock module <c>InternalSpeed</c>. In
/// case of this module is deprecated or the tags are changed, the default English values will be
/// used for the unit names.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatFixed"/></example>
public sealed class VelocityType {
  /// <summary>Localization tag for the "meter per second" units.</summary>
  public const string MeterPerSecondLocTag = "#autoLOC_180095";

  /// <summary>Localization tag for the "kilometer per second" units.</summary>
  public const string KilometerPerSecondLocTag = "#autoLOC_180103";

  /// <summary>Localization tag for the "megameter per second" units.</summary>
  public const string MegameterPerSecondLocTag = "#autoLOC_180098";

  /// <summary>Localized suffix for the "meter per second" units. Scale x1.</summary>
  public static readonly Message meterPerSecond = new Message(
      MeterPerSecondLocTag,
      defaultTemplate: " m/s",
      description: "Meter per second unit for a velocity value");

  /// <summary>Localized suffix for the "kilometer per second" units. Scale x1000.</summary>
  public static readonly Message kilometerPerSecond = new Message(
      KilometerPerSecondLocTag,
      defaultTemplate: " km/s",
      description: "Kilometer per second unit for a velocity value");

  /// <summary>Localized suffix for the "megameter per second" units. Scale x1000000.</summary>
  public static readonly Message megameterPerSecond = new Message(
      MegameterPerSecondLocTag,
      defaultTemplate: " Mm/s",
      description: "Megameter per second unit for a velocity value");

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an object from a numeric value.</summary>
  /// <param name="value">The numeric value in the base units.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatFixed"/></example>
  public VelocityType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator VelocityType(double value) {
    return new VelocityType(value);
  }

  /// <summary>Converts a type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(VelocityType obj) {
    return obj.value;
  }

  /// <summary>Formats the value into a human friendly string with a unit specification.</summary>
  /// <remarks>
  /// <para>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </para>
  /// <para>
  /// The base velocity unit in the game is <i>m/s</i>. I.e. value <c>1.0</c> in the game
  /// units is <i>one meter per second</i>. Keep it in mind when passing the argument.
  /// </para>
  /// </remarks>
  /// <param name="value">The unscaled numeric value to format.</param>
  /// <param name="scale">
  /// The fixed scale to apply to the value before formatting. The formatting method can uderstand
  /// only a few scales:
  /// <list type="bullet">
  /// <item>m/s: scale=<c>1.0</c>. <i>It's a base velocity unit in the game.</i></item>
  /// <item>km/s: scale=<c>1.0e+3</c>.</item>
  /// <item>Mm/s: scale=<c>1.0e+6</c>.</item>
  /// </list>
  /// <para>
  /// The unknown scales will be rounded <i>up</i> to the closest known scale. If this parameter
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
  /// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/VelocityType-Examples.cs" region="VelocityTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, double? scale = null, string format = null) {
    // Detect the scale, and scale the value.
    string units;
    double scaledValue;
    if (format != null && !scale.HasValue) {
      scale = 1.0;  // No scale detection.
    }
    var testValue = Math.Abs(value);
    if (!scale.HasValue) {
      // Auto detect the best scale.
      if (testValue >= 1.0e+6) {
        scale = 1.0e+6; 
      } else if (testValue >= 1.0e+3) {
        scale = 1.0e+3; 
      } else {
        scale = 1.0;
      }
    }
    if (scale >= 1.0e+6) {
      scaledValue = value / 1.0e+6;
      units = megameterPerSecond;
    } else if (scale >= 1.0e+3) {
      scaledValue = value / 1.0e+3;
      units = kilometerPerSecond;
    } else {
      scaledValue = value;
      units = meterPerSecond;
    }
    if (format != null) {
      return scaledValue.ToString(format) + units;
    }
    return CompactNumberType.Format(scaledValue) + units;
  }

  /// <summary>Returns a string formatted as a human friendly pressure specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
