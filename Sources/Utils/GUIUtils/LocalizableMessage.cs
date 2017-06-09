// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using KSP.Localization;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Base class for the messages that support localization.</summary>
/// <remarks>
/// This class is not intended for the use on its own. See how the other classes (e.g.
/// <see cref="Message"/> or <see cref="Message&lt;T&gt;"/>) are inheriting it.
/// </remarks>
/// <seealso cref="Message"/>
/// <seealso cref="Message&lt;T&gt;"/>
public class LocalizableMessage {
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
  /// <value>A tag in the language file.</value>
  /// <include file="KSPAPI_HelpIndex.xml" path="//item[@name='T:KSP.Localization.Localizer']"/>
  public string tag { get; private set; }

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
      if (_localizedTemplate == null) {
        LoadLocalization();
      }
      return _localizedTemplate;
    }
  }
  string _localizedTemplate;

  /// <summary>Instructs the implementation to load a localized template.</summary>
  /// <remarks>If there is a value cached, it will be reloaded.</remarks>
  public virtual void LoadLocalization() {
    if (!Localizer.TryGetStringByTag(tag, out _localizedTemplate)) {
      _localizedTemplate = defaultTemplate;
      Debug.LogWarningFormat("Cannot find localized content for: tag={0}, lang={1}",
                             tag, Localizer.CurrentLanguage);
    }
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
  /// <seealso href="http://lingoona.com/cgi-bin/grammar#l=en&amp;oh=1">Lingoona Grammar help</seealso>
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
