// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>Standard localization tag specification.</summary>
/// <remarks>The specification tells which property of the localizable entity to affect.</remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <seealso cref="LocalizationLoader.LoadItemsInModule"/>
public static class StdSpecTags {
  /// <summary>
  /// Specification for the <see cref="KSPField"/> <c>guiUnits</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="KSPDev.GUIUtils.LocalizableItemAttribute"/> for a field
  /// with the units.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_WithUnits"/></example>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string Units = "units";
}

}  // namespace
