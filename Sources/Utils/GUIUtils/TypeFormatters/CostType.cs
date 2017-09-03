// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a numeric value that represents a <i>cost</i>.
/// </summary>
/// <remarks>
/// <para>
/// Use it as a generic parameter when creating a <c>KSPDev.GUIUtils.LocalizableMessage</c>
/// descendants.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatDefault"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatFixed"/></example>
public sealed class CostType {
  /// <summary>A wrapped numeric value.</summary>
  /// <remarks>This is the original non-rounded and unscaled value.</remarks>
  public readonly double value;

  /// <summary>Constructs an object from a numeric value.</summary>
  /// <param name="value">The numeric value in the base units.</param>
  /// <seealso cref="Format"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatFixed"/></example>
  public CostType(double value) {
    this.value = value;
  }

  /// <summary>Coverts a numeric value into a type object.</summary>
  /// <param name="value">The numeric value to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator CostType(double value) {
    return new CostType(value);
  }

  /// <summary>Converts a type object into a numeric value.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A numeric value.</returns>
  public static implicit operator double(CostType obj) {
    return obj.value;
  }

  /// <summary>Formats the value into a human friendly string.</summary>
  /// <remarks>
  /// <para>
  /// The method tries to keep the resulted string meaningful and as short as possible. For this
  /// reason the big values may be scaled down and/or rounded.
  /// </para>
  /// <para>
  /// There is no well established unit for measuring the costs in the game. However, there is a
  /// commonly used literal that denotes the costs: <c>√</c>. This literal is used as the base
  /// (and the only) unit name. It's assumed that the values below <c>0.01</c> don't makes sense
  /// from the financial perspective, so they are not attempted to be presented.
  /// </para>
  /// </remarks>
  /// <param name="value">The numeric value to format.</param>
  /// <param name="format">
  /// The specific float number format to use. If the format is not specified, then it's choosen
  /// basing on the value.
  /// </param>
  /// <returns>A formatted and localized string</returns>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatDefault"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/CostType-Examples.cs" region="CostTypeDemo2_FormatFixed"/></example>
  public static string Format(double value, string format = null) {
    return "√ " + value.ToString("#,##0.00");  // Simulate the editor's behavior.
  }

  /// <summary>Returns a string formatted as a human friendly volume specification.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format"/>
  public override string ToString() {
    return Format(value);
  }
}

}  // namespace
