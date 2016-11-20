// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace KSPDev.GUIUtils {

/// <summary>A class to wrap a UI string for an enum value.</summary>
/// <remarks>
/// <para>
/// When string needs to be presented use <see cref="Format"/> to make the parameter substitute.
/// </para>
/// <para>
/// In the future it may support localization but for now it's only a convinience wrapper.
/// </para>
/// </remarks>
/// <example>
/// Instead of doing switches when an enum value should be presented on UI just define a message
/// that declares a map between values and their UI representations. You don't need specify every
/// single value in the map, there is an option to set a UI string for unknown value.  
/// <code><![CDATA[
/// class MyMod : MonoBehaviour {
///   enum MyEnum {
///     Disabled,
///     Enabled,
///     UnusedValue1,
///     UnusedValue2,
///     UnusedValue3,
///   }
///
///   // Lookup with custom value for an unknown key.
///   static readonly MessageEnumValue<MyEnum> Msg1 =
///       new MessageEnumValue<MyEnum>("UNKNOWN") {
///         {MyEnum.Enabled, "ENABLED"},
///         {MyEnum.Disabled, "DISABLED"},
///       };
///
///   // Default lookup.
///   static readonly MessageEnumValue<MyEnum> Msg2 =
///       new MessageEnumValue<MyEnum>() {
///         {MyEnum.Enabled, "ENABLED"},
///         {MyEnum.Disabled, "DISABLED"},
///         {MyEnum.UnusedValue1, "Value1"},
///         {MyEnum.UnusedValue2, "Value2"},
///       };
///
///   void Awake() {
///     Debug.LogFormat("Localized: {0}", Msg1.Format(MyEnum.Disabled));  // DISABLED
///     Debug.LogFormat("Localized: {0}", Msg1.Format(MyEnum.UnusedValue1));  // UNKNOWN
///
///     Debug.LogFormat("Localized: {0}", Msg2.Format(MyEnum.UnusedValue1));  // Value1
///     Debug.LogFormat("Localized: {0}", Msg2.Format(MyEnum.UnusedValue2));  // Value2
///     Debug.LogFormat("Localized: {0}", Msg2.Format(MyEnum.UnusedValue3));  // "" (null)
///   }
/// }
/// ]]></code>
/// </example>
public class MessageEnumValue<T> : IEnumerable<KeyValuePair<T, string>> {
  readonly Dictionary<T, string> strings;
  readonly string unknownKeyValue;

  /// <summary>Creates an empty message with a default value for unknow entries.</summary>
  /// <param name="unknownKeyValue">
  /// Value to return if lookup dictionary doesn't have the requested key.
  /// </param>
  public MessageEnumValue(string unknownKeyValue = null) {
    this.strings = new Dictionary<T, string>();
    this.unknownKeyValue = unknownKeyValue;
  }

  /// <inheritdoc/>
  public IEnumerator<KeyValuePair<T, string>> GetEnumerator() {
    return strings.GetEnumerator();
  }

  /// <summary>Adds a new lookup for the key.</summary>
  /// <param name="key">Unique key.</param>
  /// <param name="value">GUI string for the key.</param>
  public void Add(T key, string value) {
    strings.Add(key, value);
  }

  /// <summary>Formats message string with the provided arguments.</summary>
  /// <param name="arg1">An argument to substitute.</param>
  /// <returns>Complete message string.</returns>
  public string Format(T arg1) {
    string value;
    return strings.TryGetValue(arg1, out value) ? value : unknownKeyValue;
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}

}  // namespace KSPDev.GUIUtils
