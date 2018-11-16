// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Hermetic GUI control for the enum types.</summary>
/// <remarks>
/// Use it when a cheap dropdown control needed. It's not good to work with too many options,
/// though. The rule of thumb: 2-7 options are OK.
/// </remarks>
public sealed class HermeticGUIControlSwitch : AbstractHermeticGUIControl {

  #region Initialization settings.
  readonly bool useOwnLayout;
  readonly Func<object, string> toStringConverter;
  readonly object[] valueOptions;
  #endregion

  #region IRenderableGUIControl implementation
  /// <inheritdoc/>
  public override void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions) {
    if (useOwnLayout) {
      GUILayout.BeginHorizontal(layoutStyle);
    }
    var value = GetMemberValue<object>();
    var idx = 0;
    if (GUILayout.Button("<", GUILayout.ExpandWidth(false))) {
      idx = -1;
    }
    var centeredTextStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
    GUILayout.Label(toStringConverter(value), centeredTextStyle, layoutOptions);
    if (GUILayout.Button(">", GUILayout.ExpandWidth(false))) {
      idx = 1;
    }
    if (useOwnLayout) {
      GUILayout.EndHorizontal();
    }

    var newValue = value;
    if (idx != 0) {
      var pos = valueOptions.IndexOf(value);
      if (pos == -1) {
        pos = 0;
      }
      newValue = valueOptions[(valueOptions.Length + pos + idx) % valueOptions.Length];
    }
    if (!newValue.Equals(value)) {
      SetMemberValue(newValue, actionsList);
    }
  }
  #endregion

  /// <summary>Creates a control, bound to a field.</summary>
  /// <param name="instance">The class instance that owns the field to control.</param>
  /// <param name="fieldInfo">The field information of thefield to control.</param>
  /// <param name="onUpdate">The callback to call when the value is changed.</param>
  /// <param name="useOwnLayout">
  /// If <c>false</c>, then the control will start own horizontal section to align the input field
  /// and buttons.
  /// </param>
  /// <param name = "toStringConverter">
  /// The function to use to map the enum values into a human friendly strings. By default, the enum
  /// value is simply converted into string via <c>ToString()</c>.
  /// </param>
  /// <seealso cref="ConfigUtils.StandardOrdinaryTypesProto"/>
  public HermeticGUIControlSwitch(object instance, FieldInfo fieldInfo,
                                  Action onUpdate = null, bool useOwnLayout = true,
                                  Func<object, string> toStringConverter = null)
      : base(instance, fieldInfo, onUpdate: onUpdate) {
    this.useOwnLayout = useOwnLayout;
    this.toStringConverter = toStringConverter != null ? toStringConverter : x => x.ToString();
    if (!GetMemberType().IsEnum) {
      throw new ArgumentException(string.Format(
          "Unsupported type: type={0}", GetMemberType()));
    }
    valueOptions = Enum.GetValues(GetMemberType()).Cast<object>().ToArray();
  }
}

}  // namespace
