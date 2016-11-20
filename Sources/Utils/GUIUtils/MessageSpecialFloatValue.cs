// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

namespace KSPDev.GUIUtils {

/// <summary>
/// A class to wrap a UI string with one parameter which may have special meaning.
/// </summary>
/// <remarks>
/// <para>When string needs to be presented use <see cref="Format"/> to make the parameter
/// substitute.</para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of presenting hardcoded strings on UI move them all into a special section, and assign
/// to fields of type <c>Message</c>.
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   MessageSpecialFloatValue MyMessage =
///       new MessageSpecialFloatValue("Param: {0}", 0, "Param is ZERO");
///
///   void Awake() {
///     Debug.LogFormat("Localized: {0}", MyMessage.Format(1));  // Param: 1
///     Debug.LogFormat("Localized: {0}", MyMessage.Format(0));  // Param is ZERO
///   }
/// }
/// ]]></code>
/// </example>
public class MessageSpecialFloatValue {
  readonly string fmtString;
  readonly string specialValueString;
  readonly float specialValue;
  
  /// <summary>Creates a message.</summary>
  /// <param name="fmtString">A message format string.</param>
  /// <param name="specialValue">Value to use a special message string for.</param>
  /// <param name="specialString">Special message string for the value.</param>
  public MessageSpecialFloatValue(string fmtString, float specialValue, string specialString) {
    this.fmtString = fmtString;
    this.specialValueString = specialString;
    this.specialValue = specialValue;
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(float arg1) {
    if (UnityEngine.Mathf.Approximately(arg1, specialValue)) {
      return specialValueString;
    }
    return string.Format(fmtString, arg1);
  }
}

}  // namespace
