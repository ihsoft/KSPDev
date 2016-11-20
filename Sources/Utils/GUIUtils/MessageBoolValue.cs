// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils {

/// <summary>A class to wrap a UI string for a boolean value.</summary>
/// <remarks>
/// <para>
/// When string needs to be presented use <see cref="Format"/> to make the parameter substitute.
/// </para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>MessageBoolValue</c>.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   static readonly MessageBoolValue SwitchMsg = new MessageBoolValue("ON", "OFF");
///   static readonly MessageBoolValue StateMsg = new MessageBoolValue("Enabled", "Disabled");
///
///   void Awake() {
///     Debug.LogFormat("Localized: {0}", SwitchMsg.Format(true));  // ON
///     Debug.LogFormat("Localized: {0}", StateMsg.Format(false));  // Disabled
///   }
/// }
/// ]]></code>
/// </example>
public class MessageBoolValue {
  readonly string positiveStr;
  readonly string negativeStr;
  
  /// <summary>Creates a message.</summary>
  /// <param name="positiveStr">Message string for <c>true</c> value.</param>
  /// <param name="negativeStr">Message string for <c>false</c> value.</param>
  public MessageBoolValue(string positiveStr, string negativeStr) {
    this.positiveStr = positiveStr;
    this.negativeStr = negativeStr;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(bool arg1) {
    return arg1 ? positiveStr : negativeStr;
  }
}

}  // namespace KSPDev.GUIUtils
