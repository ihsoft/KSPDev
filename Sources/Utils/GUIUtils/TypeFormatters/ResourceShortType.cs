// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ResourceUtils;

namespace KSPDev.GUIUtils {

/// <summary>
/// Localized message formatting class for a string value that represents an
/// <i>abbreviated resource</i> type.
/// </summary>
/// <remarks>
/// <para>
/// This class resolves the resource type/ID into it's <i>short</i> localized name. It's usually 2
/// letters long, but it's not mandatory.
/// </para>
/// <para>
/// Use it as a generic parameter when creating a <see cref="LocalizableMessage"/> descendants.
/// </para>
/// </remarks>
/// <include file="SpecialDocTags.xml" path="Tags/MessageTypeWithArg/*"/>
/// <include file="SpecialDocTags.xml" path="Tags/MessageArgumentType/*"/>
/// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo1"/></example>
/// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo2_FormatDefault"/></example>
public sealed class ResourceShortType {
  /// <summary>A wrapped resource ID value.</summary>
  public readonly int resourceId;

  /// <summary>Constructs an object from a resource nane.</summary>
  /// <param name="resourceName">The resource type name.</param>
  /// <seealso cref="Format(string)"/>
  /// <seealso cref="Format(int)"/>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo2_FormatDefault"/></example>
  public ResourceShortType(string resourceName) {
    resourceId = StockResourceNames.GetId(resourceName);
  }

  /// <summary>Constructs an object from a resource ID.</summary>
  /// <param name="resourceId">The resource ID.</param>
  /// <seealso cref="Format(string)"/>
  /// <seealso cref="Format(int)"/>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo2_FormatDefault"/></example>
  public ResourceShortType(int resourceId) {
    this.resourceId = resourceId;
  }

  /// <summary>Coverts a resource name value into a type object.</summary>
  /// <param name="value">The name to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator ResourceShortType(string value) {
    return new ResourceShortType(value);
  }

  /// <summary>Coverts a resource ID value into a type object.</summary>
  /// <param name="value">The ID to convert.</param>
  /// <returns>An object.</returns>
  public static implicit operator ResourceShortType(int value) {
    return new ResourceShortType(value);
  }

  /// <summary>Converts a type object into a resource name.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A string type value.</returns>
  public static implicit operator string(ResourceShortType obj) {
    return PartResourceLibrary.Instance.GetDefinition(obj.resourceId).name;
  }

  /// <summary>Converts a type object into a resource ID.</summary>
  /// <param name="obj">The object type to convert.</param>
  /// <returns>A string type value.</returns>
  public static implicit operator int(ResourceShortType obj) {
    return obj.resourceId;
  }

  /// <summary>Formats the value into a human friendly localized string.</summary>
  /// <param name="resourceName">The name of the resource to format.</param>
  /// <returns>A formatted and localized string.</returns>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo2_FormatDefault"/></example>
  public static string Format(string resourceName) {
    return StockResourceNames.GetResourceAbbreviation(resourceName);
  }

  /// <summary>Formats the value into a human friendly localized string.</summary>
  /// <param name="resourceId">The ID of the resource to format.</param>
  /// <returns>A formatted and localized string.</returns>
  /// <example><code source="Examples/GUIUtils/ResourceShortType-Examples.cs" region="ResourceShortTypeDemo2_FormatDefault"/></example>
  public static string Format(int resourceId) {
    return StockResourceNames.GetResourceAbbreviation(resourceId);
  }

  /// <summary>Returns a string formatted as a human friendly resource name.</summary>
  /// <returns>A string representing the value.</returns>
  /// <seealso cref="Format(int)"/>
  public override string ToString() {
    return Format(resourceId);
  }
}

}  // namespace
