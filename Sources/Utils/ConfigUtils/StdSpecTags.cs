// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>Standard specialization tags.</summary>
/// <remarks>The specialization tells which property of the localizable entity to affect.</remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <seealso cref="LocalizationLoader.LoadItemsInModule"/>
public static class StdSpecTags {
  /// <summary>
  /// Specialization for the <see cref="KSPField"/> <c>guiUnits</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="KSPDev.GUIUtils.LocalizableItemAttribute"/> for a field
  /// with the units.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_WithUnits"/></example>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string Units = "units";

  /// <summary>
  /// Specialization for the <see cref="UI_Toggle"/> <c>enabledText</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="KSPDev.GUIUtils.LocalizableItemAttribute"/> for a toggle
  /// field.
  /// </remarks>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string ToggleEnabled = "toggleEnabled";

  /// <summary>
  /// Specialization for the <see cref="UI_Toggle"/> <c>disabledText</c> localization. 
  /// </summary>
  /// <remarks>
  /// Use it when specifying a <see cref="KSPDev.GUIUtils.LocalizableItemAttribute"/> for a toggle
  /// field.
  /// </remarks>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSPField']"/>
  public const string ToggleDisabled = "toggleDisabled";
}

}  // namespace
