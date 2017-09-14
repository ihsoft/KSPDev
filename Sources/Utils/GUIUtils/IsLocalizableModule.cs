// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.KSPInterfaces;  // For the XML docs.

namespace KSPDev.GUIUtils {

/// <summary>Generic interface for the modules that manage own localizable items.</summary>
/// <remarks>
/// The "item" can be anything. If the module pays special attention to deal with the "items", then
/// it should implement this interface so that the notification could be sent when the language has
/// changed (or updated). Here are some examples of the localizable items:
/// <list type="bullet">
/// <item>
/// KSP events, actions or fields that are attributed with <see cref="LocalizableItemAttribute"/>.
/// The module would want to call the <see cref="LocalizationLoader.LoadItemsInModule"/> method to
/// have the strings updated.
/// </item>
/// <item>Cached strings. The module would want to refresh the cache.</item>
/// <item>
/// Dynamically created part menu items. The module would want to recreate them. If that's the case
/// it's always better to implement <see cref="IHasContextMenu"/>, and then just call the update
/// method.
/// </item>
/// </list>
/// </remarks>
/// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo2"/></example>
public interface IsLocalizableModule {
  /// <summary>A callback which is called when the localization vesion has changed.</summary>
  /// <remarks>
  /// Unless the implementing class is <i>sealed</i>, the method must be declared as <i>virtual</i>.
  /// The descendants may want to react on the callback as well.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizationLoader-Examples.cs" region="LocalizationLoaderDemo2"/></example>
  void LocalizeModule();
}

}  // namespace
