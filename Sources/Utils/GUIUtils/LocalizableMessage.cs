// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSP.Localization;
using KSPDev.LogUtils;
using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Base class for the messages that support localization.</summary>
/// <remarks>
/// <para>
/// This class is not intended for the use on its own. See the inheritance hierarchy for the classes
/// that inherit it.
/// </para>
/// <para>
/// This class is performance optimized. Once a string is resolved to the localized content, it's
/// cached and reused in the subsequent calls. The cache can be reset by incrementing the
/// <see cref="systemLocVersion"/>.
/// </para>
/// <para>
/// The template of the messages supports special tags that may give a hint to the caller code on
/// how the messages should be rendered. Those tags must be palced at the beginning of the template.
/// For the available tags see <see cref="GuiTags"/>.
/// </para>
/// </remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <seealso cref="systemLocVersion"/>
public class LocalizableMessage {
  /// <summary>Various values that give hints on how the messages should be presented in GUI.</summary>
  /// <remarks>
  /// It's up to the caller to handle theses settings. They improve the appearence, but are not
  /// required for the proper content presentation.
  /// </remarks>
  public class GuiTags {
    /// <summary>Minimum width of the area in GUI.</summary>
    /// <remarks>Defined via tag: &lt;gui:min:width,heigth&gt;</remarks>
    public float minWidth = 0;

    /// <summary>Minimum height of the area in GUI.</summary>
    /// <remarks>Defined via tag: &lt;gui:min:width,heigth&gt;</remarks>
    public float minHeight = 0;

    /// <summary>Maximum width of the area in GUI.</summary>
    /// <remarks>Defined via tag: &lt;gui:max:width,heigth&gt;</remarks>
    public float maxWidth = float.PositiveInfinity;

    /// <summary>Maximum height of the area in GUI.</summary>
    /// <remarks>Defined via tag: &lt;gui:max:width,heigth&gt;</remarks>
    public float maxHeight = float.PositiveInfinity;
  }

  /// <summary>Current version of the loaded localizations.</summary>
  /// <remarks>Increase it to have all the caches to invalidate.</remarks>
  /// <seealso cref="localizedTemplate"/>
  /// <seealso cref="loadedLocVersion"/>
  public static int systemLocVersion = 1;

  /// <summary>Template to use if no localized template found.</summary>
  public readonly string defaultTemplate;

  /// <summary>Description for the message.</summary>
  /// <remarks>
  /// This string is not presented anywhere in the game, but it can be presented to the people who
  /// will be translating the template into a different language.
  /// </remarks>
  public readonly string description;

  /// <summary>Example usage and the output.</summary>
  /// <remarks>
  /// This string is not presented anywhere in the game, but it can be presented to the people who
  /// will be translating the template into a different language.
  /// </remarks>
  public readonly string example;

  /// <summary>Tag to use when resolving the string via the Localizer.</summary>
  /// <remarks>
  /// It can be <c>null</c> to indicate that the localization is not needed. In this case the
  /// <see cref="defaultTemplate"/> will be used as text.
  /// </remarks>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSP.Localization.Localizer']"/>
  public readonly string tag;

  /// <summary>GUI specific settings that suggest how to show the message.</summary>
  /// <remarks>
  /// Due to the lazzy update nature of the localized messages, these settings are <i>not</i>
  /// loaded until the message is used at least once. The caller code may ensure the values are
  /// updated by calling to <see cref="LoadLocalization"/>, or by simply getting the
  /// <see cref="localizedTemplate"/> value.
  /// </remarks>
  public GuiTags guiTags = new GuiTags();

  /// <summary>Localized Lingoona Grammar template for the <c>tag</c>.</summary>
  /// <remarks>
  /// <para>
  /// The template is resolved via the Localizer only once. The resolved value is cached and
  /// re-sued.
  /// </para>
  /// <para>
  /// If there is no string defined for the tag, then a <see cref="defaultTemplate"/> will be used.
  /// A warning record will be logged to help tracking such issues.
  /// </para>
  /// <para>
  /// When current language is changed the cached version needs to be reloaded. Call
  /// the <see cref="LoadLocalization"/> method to force it. However, as of KSP 1.3.0 the langauge
  /// cannot be changed while the game is running.
  /// </para>
  /// </remarks>
  /// <value>
  /// A Lingoona Grammar template in the
  /// <see cref="Localizer.CurrentLanguage">current languge</see>.
  /// </value>
  /// <seealso cref="LoadLocalization"/>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSP.Localization.Localizer']"/>
  public string localizedTemplate {
    get {
      if (loadedLocVersion != systemLocVersion) {
        LoadLocalization();
      }
      return GameSettings.SHOW_TRANSLATION_KEYS_ON_SCREEN ? tag : _localizedTemplate;
    }
  }
  string _localizedTemplate;

  /// <summary>Currently cached version of the localization content.</summary>
  /// <remarks>
  /// If this version is different from the <see cref="systemLocVersion"/>, then the message
  /// <see cref="localizedTemplate"/> will be reloaded from the config when accessed.
  /// </remarks>
  /// <seealso cref="localizedTemplate"/>
  /// <seealso cref="systemLocVersion"/>
  protected int loadedLocVersion;

  /// <summary>Instructs the implementation to load a localized template.</summary>
  /// <remarks>If there is a value cached, it will be reloaded.</remarks>
  public virtual void LoadLocalization() {
    if (tag == null || !Localizer.TryGetStringByTag(tag, out _localizedTemplate)) {
      _localizedTemplate = defaultTemplate;
      if (tag != null && GameSettings.LOG_MISSING_KEYS_TO_FILE) {
        DebugEx.Warning("Cannot find localized content for: tag={0}, lang={1}",
                        tag, Localizer.CurrentLanguage);
      }
    }
    HandleSpecialTags();
    loadedLocVersion = systemLocVersion;
  }

  /// <summary>Constructs a localizable message.</summary>
  /// <param name="tag">
  /// The tag to use when getting the localized version of the template. If <c>null</c> then the
  /// message will alaways use <paramref name="defaultTemplate"/> as text.
  /// </param>
  /// <param name="defaultTemplate">
  /// <para>
  /// The template to use if no localizable content can be found. It can be in any language, but
  /// it's strongly encouraged to put it in English. For the template syntax see the documentation
  /// on the Lingoona website.
  /// </para>
  /// <para>
  /// If this parameter is omitted, then the <paramref name="tag"/> will be used as a default
  /// template. I.e. in case of the tag lookup failed, the tag string will be presented instead of
  /// a human readable string.
  /// </para>
  /// </param>
  /// <param name="description">
  /// A helper text for the translators to give them a context. Try to be reasonably verbose when
  /// specifying the circumstances of when this string is displayed. The context <i>does</i> matter!
  /// </param>
  /// <param name="example">
  /// An example of how the template can be used and what is the output in the langauge of the
  /// <paramref name="defaultTemplate"/>. Provide it to illustrate the non-obvious cases. 
  /// </param>
  /// <include file="SpecialDocTags.xml" path="Tags/Lingoona/*"/>
  protected LocalizableMessage(string tag,
                               string defaultTemplate = null,
                               string description = null, string example = null) {
    this.tag = tag;
    this.defaultTemplate = defaultTemplate ?? tag ?? "";
    this.description = description ?? "";
    this.example = example ?? "";
  }

  /// <summary>Handles any special tags that can prefix the actual template.</summary>
  void HandleSpecialTags() {
    guiTags = new GuiTags();  // Reset to default.
    while (_localizedTemplate.StartsWith("<gui:", StringComparison.Ordinal)) {
      var endOfSpecial = _localizedTemplate.IndexOf('>');
      if (endOfSpecial == -1) {
        DebugEx.Error("[{0}] Bad syntax of the GUI settings specification: {1}",
                      tag, _localizedTemplate);
        return;
      }
      var special = _localizedTemplate.Substring(5, endOfSpecial - 5);
      _localizedTemplate = _localizedTemplate.Substring(endOfSpecial + 1);
      var endOfTag = special.IndexOf(':');
      if (endOfTag == -1) {
        DebugEx.Error("[{0}] Bad syntax of the GUI special tag: {1}", tag, special);
        return;
      }
      var specialTag = special.Substring(0, endOfTag);
      var specialValue = special.Substring(endOfTag + 1);
      if (specialTag == "min") {
        var minSize = ConfigNode.ParseVector2(specialValue);
        guiTags.minWidth = minSize.x;
        guiTags.minHeight = minSize.y;
      } else if (specialTag == "max") {
        var minSize = ConfigNode.ParseVector2(specialValue);
        guiTags.maxWidth = minSize.x;
        guiTags.maxHeight = minSize.y;
      } 
    }
  }
}

}  // namespace
