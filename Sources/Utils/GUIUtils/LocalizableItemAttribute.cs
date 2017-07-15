// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils {

/// <summary>Attribute for the various game items that support localization.</summary>
/// <remarks>
/// <para>
/// Use this attribute to automatically localize the part's events, fields, and actions. The
/// relevant strings will be loaded when the vessel or part is either created or loaded. The
/// strings will also properly update when reloading the language definitiaion via
/// <c>LocalizationTool</c>.
/// </para>
/// <para>
/// See the "seealso" section for the types and methods that are aware of this attribute.
/// </para>
/// </remarks>
/// <seealso cref="LocalizableMessage"/>
/// <seealso cref="LocalizationLoader.LoadItemsInModule"/>
/// <seealso href="https://github.com/ihsoft/KSPDev/tree/master/Sources/LocalizationTool">KSPDev: LocalizationTool</seealso>
/// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField"/></example>
/// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemEvent"/></example>
/// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemAction"/></example>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method,
                AllowMultiple = true)]
public class LocalizableItemAttribute : Attribute {
  /// <inheritdoc cref="LocalizableMessage.defaultTemplate"/>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField"/></example>
  public string defaultTemplate;

  /// <inheritdoc cref="LocalizableMessage.description"/>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField"/></example>
  public string description;

  /// <inheritdoc cref="LocalizableMessage.tag"/>
  /// <remarks>
  /// It can be set to <c>null</c> to indicate that the attributed item doesn't need localization.
  /// Such attributes are NO-OP during the game, but for the localization tool it's a signal to not
  /// include the item into the localization config.
  /// </remarks>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField"/></example>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_NullTag"/></example>
  public string tag;

  /// <summary>
  /// A key to distinguish attributes when there are multiple of them attached to a member. 
  /// </summary>
  /// <remarks>The meaning of this key depends on the attributed member.</remarks>
  /// <seealso cref="LocalizationLoader.LoadItemsInModule"/>
  /// <example><code source="Examples/GUIUtils/LocalizableItemAttribute-Examples.cs" region="ItemField_WithUnits"/></example>
  public string spec;

  /// <summary>Cached localized message.</summary>
  Message localizedString;

  /// <summary>Returns the localized string.</summary>
  /// <remarks>
  /// This method is primary designed for the KSPDev localization code. There are no good use cases
  /// for the application code to invoke it.
  /// </remarks>
  /// <returns>
  /// The localized string of the <see cref="defaultTemplate"/> if no localization content found.
  /// </returns>
  public string GetLocalizedString() {
    if (localizedString == null) {
      localizedString =
          new Message(tag, defaultTemplate: defaultTemplate, description: description);
    }
    return localizedString;
  }
}

}  // namespace
