// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSP.Localization;
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
/// cached and reused in the subsequent calls. The cache can be reset by inceremnting the
/// <see cref="systemLocVersion"/>.
/// </para>
/// </remarks>
/// <seealso cref="LocalizableItemAttribute"/>
/// <seealso cref="systemLocVersion"/>
public class LocalizableMessage {
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
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSP.Localization.Localizer']"/>
  public readonly string tag;

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
    if (!Localizer.TryGetStringByTag(tag, out _localizedTemplate)) {
      _localizedTemplate = defaultTemplate;
      if (GameSettings.LOG_MISSING_KEYS_TO_FILE) {
        Debug.LogWarningFormat("Cannot find localized content for: tag={0}, lang={1}",
                               tag, Localizer.CurrentLanguage);
      }
    }
    loadedLocVersion = systemLocVersion;
  }

  /// <summary>Constructs a localizable message.</summary>
  /// <param name="tag">The tag to use when getting the localized version of the template.</param>
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
    if (tag == null) {
      throw new ArgumentException("The tag cannot be NULL");
    }
    this.tag = tag;
    this.defaultTemplate = defaultTemplate ?? tag;
    this.description = description ?? "";
    this.example = example ?? "";
  }
}

}  // namespace
