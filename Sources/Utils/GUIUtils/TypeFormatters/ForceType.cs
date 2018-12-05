// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils.TypeFormatters {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>force</i>. The
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
/// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatFixed"/></example>
public sealed class ForceType {
  /// <summary>Localization tag for the "kilonewton" units.</summary>
  public const string KilonewtonLocTag = "#autoLOC_7001408";
  
  /// <summary>Localized suffix for the "kilonewton" units. Scale x1.</summary>
  public static readonly Message kiloNewton = new Message(
      KilonewtonLocTag, defaultTemplate: " kN",
      description: "Kilonewton unit for a force value");

  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an object from a numeric value.</summary>
  /// <param name="value">The numeric value in kilonewtons.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatFixed"/></example>
  public ForceType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator ForceType(double value) {
    return new ForceType(value);
  }

  /// <summary>Converts a type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(ForceType obj) {
    return obj.value;
  }

  /// <summary>Formats the value into a human friendly string with a unit specification.</summary>
  /// <remarks>
  /// <para>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </para>
  /// <para>
  /// The base force unit in the game is <i>kilonewton</i>. I.e. value <c>1.0</c> in the game
  /// units is <i>one kilonewton</i>. Keep it in mind when passing the argument.
  /// </para>
  /// </remarks>
  /// <param name="value">The numeric value to format.</param>
  /// <param name="format">
  /// The specific float number format to use. If the format is not specified, then it's choosen
  /// basing on the value.
  /// </param>
  /// <returns>A formatted and localized string</returns>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ForceType-Examples.cs" region="ForceTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, string format = null) {
    if (format != null) {
      return value.ToString(format) + kiloNewton.Format();
    }
    return CompactNumberType.Format(value) + kiloNewton.Format();
  }

  /// <summary>Returns a string formatted as a human friendly force specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
