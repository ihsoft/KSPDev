// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using System;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Hermetic GUI control for the generic types, represented as a plain text.</summary>
/// <remarks>
/// The default implementation of the control supports all the standard game types that are used in
/// the config fiels. Extra types support can be added by providing a custom value prototype
/// interface.
/// </remarks>
/// <seealso cref="AbstractOrdinaryValueTypeProto"/>
/// <seealso cref="StandardOrdinaryTypesProto"/>
public sealed class HermeticGUIControlText : AbstractHermeticGUIControl {

  #region Initialization settings.
  readonly bool useOwnLayout;
  readonly AbstractOrdinaryValueTypeProto valueTypeProto;
  #endregion

  /// <summary>Currently accumulated text input.</summary>
  string currentTxt;

  /// <summary>Tells if <see cref="currentTxt"/> can be applied to the field.</summary>
  bool isValid;

  #region IRenderableGUIControl implementation
  /// <inheritdoc/>
  public override void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions) {
    if (useOwnLayout) {
      GUILayout.BeginHorizontal(layoutStyle);
    }

    var value = GetMemberValue<object>();
    var valueTxt = valueTypeProto.SerializeToString(value);
    if (currentTxt == null) {
      currentTxt = valueTxt;
      isValid = true;
    }
    var changed = currentTxt != valueTxt;
    if (changed) {
      isValid = true;
      try {
        valueTypeProto.ParseFromString(currentTxt, GetMemberType());
      } catch (Exception) {
        isValid = false;
      }
    }
    using (new GuiColorScope(contentColor: isValid ? Color.white : Color.red)) {
      currentTxt = GUILayout.TextField(changed ? currentTxt : valueTxt, layoutOptions);
    }
    using (new GuiEnabledStateScope(changed)) {
      using (new GuiEnabledStateScope(changed && isValid)) {
        if (GUILayout.Button("S", GUILayout.ExpandWidth(false))) {
          value = valueTypeProto.ParseFromString(currentTxt, GetMemberType());
          currentTxt = valueTypeProto.SerializeToString(value);
          SetMemberValue(value);
        }
      }
      if (GUILayout.Button("C", GUILayout.ExpandWidth(false))) {
        currentTxt = valueTxt;
        isValid = true;
      }
    }
    if (useOwnLayout) {
      GUILayout.EndHorizontal();
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
  /// <param name="valueTypeProto">
  /// The value type conversion proto to use to covert the field value to/from string. By default,
  /// all the game's standard configuration types are supported, except the collections.
  /// </param>
  /// <seealso cref="ConfigUtils.StandardOrdinaryTypesProto"/>
  public HermeticGUIControlText(object instance, FieldInfo fieldInfo,
                                Action onUpdate = null, bool useOwnLayout = true,
                                AbstractOrdinaryValueTypeProto valueTypeProto = null)
      : base(instance, fieldInfo, onUpdate: onUpdate) {
    this.useOwnLayout = useOwnLayout;
    valueTypeProto = valueTypeProto ?? new StandardOrdinaryTypesProto();
    this.valueTypeProto = valueTypeProto;
    if (!valueTypeProto.CanHandle(fieldInfo.FieldType)) {
      throw new ArgumentException(string.Format(
          "Unsupported type: proto={0}, type={1}", valueTypeProto, fieldInfo.FieldType));
    }
  }
}

}  // namespace
