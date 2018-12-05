// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils.TypeFormatters {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>mass</i>. The
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
/// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatWithScale"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatFixed"/></example>
public sealed class MassType {
  /// <summary>Localization tag for the "gram" units.</summary>
  public const string GramLocTag = "#autoLOC_7001412";
  
  /// <summary>Localization tag for the "kilogram" units.</summary>
  public const string KilogramLocTag = "#autoLOC_7001403";

  /// <summary>Localization tag for the "ton" units.</summary>
  public const string TonLocTag = "#autoLOC_7001407";

  /// <summary>Localized suffix for the "gram" units. Scale <c>0.000001</c>.</summary>
  public static readonly Message gram = new Message(
      GramLocTag, defaultTemplate: " grams",
      description: "Gram unit for a mass value");

  /// <summary>Localized suffix for the "kilogram" untis. Scale <c>0.001</c></summary>
  public static readonly Message kilogram = new Message(
      KilogramLocTag, defaultTemplate: " kg",
      description: "Kilogram unit for a mass value");

  /// <summary>Localized suffix for the "ton" untis. Scale <c>1.0</c>.</summary>
  public static readonly Message ton = new Message(
      TonLocTag, defaultTemplate: " t",
      description: "Ton unit for a mass value");

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an object from a numeric value.</summary>
  /// <param name="value">The numeric value in meters.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatFixed"/></example>
  public MassType(double value) {
    this.value = value;
  }

  /// <summary>Converts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator MassType(double value) {
    return new MassType(value);
  }

  /// <summary>Converts a type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(MassType obj) {
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
  /// <item>Tons: scale=<c>1.0</c>. <i>It's a base mass unit in the game.</i></item>
  /// <item>Kilograms: scale=<c>1.0e-3</c>.</item>
  /// <item>Grams: scale=<c>1.0e-6</c>.</item>
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
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatWithScale"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/MassType-Examples.cs" region="MassTypeDemo2_FormatFixed"/></example>
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
      if (testValue < 0.001) {
        scale = 0.000001; 
      } else if (testValue < 1.0) {
        scale = 0.001; 
      } else {
        scale = 1.0;
      }
    }
    if (scale <= 0.000001) {
      scaledValue = value / 0.000001;
      units = gram;
    } else if (scale <= 0.001) {
      scaledValue = value / 0.001;
      units = kilogram;
    } else {
      scaledValue = value;
      units = ton;
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
