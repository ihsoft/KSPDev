// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.LocalizationTool {

/// <summary>A container for an item that needs localization.</summary>
struct LocItem {
  /// <summary>The physical file path contains the entity declaration.</summary>
  public string fullFilePath;

  /// <summary>A string to use to group the similar items together.</summary>
  /// <remarks>It can be anything, but usually it's something meaningful.</remarks>
  public string groupKey;

  /// <summary>A string to use to sort the items within the group.</summary>
  /// <remarks>
  /// The items are always sorted by this value first. Then, an extra sorting can be applied
  /// depening on the writing preferences.
  /// </remarks>
  public string sortKey;

  /// <summary>The tag to use for resolution of the localized content.</summary>
  public string locTag;

  /// <summary>A value to use when the localization is not available.</summary>
  /// <remarks>
  /// In some cases it can be the localized value for the current game language. E.g. the KSP
  /// annotated fields only give <c>guiName</c> in a localized form, i.e. if the value has been
  /// set to a tag, and the tag is known to the game, then the localized value will be returned
  /// instead of the tag (which was the original value).
  /// </remarks>
  public string locDefaultValue;

  /// <summary>Optional description of the item.</summary>
  /// <remarks>
  /// It can be different in every particular case. The general rule is that this field should
  /// give a context which is absent otherwsie (e.g. via the tag name or via the default value). 
  /// </remarks>
  public string locDescription;

  /// <summary>Optional example usage of the template.</summary>
  /// <remarks>
  /// The example must be provided by the localization class. E.g.
  /// <see cref="KSPDev.GUIUtils.LocalizableMessage"/>.
  /// </remarks>
  public string locExample;
}

}  // namespace
