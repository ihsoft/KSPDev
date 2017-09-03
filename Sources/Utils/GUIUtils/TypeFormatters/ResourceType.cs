// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ResourceUtils;

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a string value that represents a <i>resource</i> type.
/// </summary>
/// <remarks>
/// <para>This class resolves the resource type/ID into it's full localized name.</para>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <seealso cref="StockResourceNames"/>
/// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo2_FormatDefault"/></example>
public sealed class ResourceType {
  /// <summary>A wrapped resource ID value.</summary>
  public readonly int resourceId;

  /// <summary>Constructs an object from a resource nane.</summary>
  /// <param name="resourceName">The resource type name.</param>
  /// <seealso cref="Format(int)"/>
  /// <seealso cref="Format(string)"/>
  /// <seealso cref="StockResourceNames"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo2_FormatDefault"/></example>
  public ResourceType(string resourceName) {
    resourceId = StockResourceNames.GetId(resourceName);
  }

  /// <summary>Constructs an object from a resource ID.</summary>
  /// <param name="resourceId">The resource ID.</param>
  /// <seealso cref="Format(int)"/>
  /// <seealso cref="Format(string)"/>
  /// <seealso cref="StockResourceNames"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo2_FormatDefault"/></example>
  public ResourceType(int resourceId) {
    this.resourceId = resourceId;
  }

  /// <summary>Coverts a resource name value into a type object.</summary>
  /// <param name="value">The name to convert.</param>
  /// <returns>An object.</returns>
  /// <seealso cref="StockResourceNames"/>
  public static implicit operator ResourceType(string value) {
    return new ResourceType(value);
  }

  /// <summary>Coverts a resource ID value into a type object.</summary>
  /// <param name="value">The ID to convert.</param>
  /// <returns>An object.</returns>
  /// <seealso cref="StockResourceNames"/>
  public static implicit operator ResourceType(int value) {
    return new ResourceType(value);
  }

  /// <summary>Converts a type object into a resource name.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A string type value.</returns>
  public static implicit operator string(ResourceType obj) {
    return PartResourceLibrary.Instance.GetDefinition(obj.resourceId).name;
  }

  /// <summary>Converts a type object into a resource ID.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A string type value.</returns>
  public static implicit operator int(ResourceType obj) {
    return obj.resourceId;
  }

  /// <summary>Formats the value into a human friendly localized string.</summary>
  /// <param name="resourceName">The name of the resource to format.</param>
  /// <returns>A formatted and localized string.</returns>
  /// <seealso cref="StockResourceNames"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo2_FormatDefault"/></example>
  public static string Format(string resourceName) {
    return StockResourceNames.GetResourceTitle(resourceName);
  }

  /// <summary>Formats the value into a human friendly localized string.</summary>
  /// <param name="resourceId">The ID of the resource to format.</param>
  /// <returns>A formatted and localized string.</returns>
  /// <seealso cref="StockResourceNames"/>
  /// <example><code source="Examples/GUIUtils/TypeFormatters/ResourceType-Examples.cs" region="ResourceTypeDemo2_FormatDefault"/></example>
  public static string Format(int resourceId) {
    return StockResourceNames.GetResourceTitle(resourceId);
  }

  /// <summary>Returns a string formatted as a human friendly resource name.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format(int)"/>
  public override string ToString() {
    return Format(resourceId);
  }
}

}  // namespace
