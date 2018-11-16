// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Hermetic GUI control for the boolean members.</summary>
public sealed class HermeticGUIControlBoolean : AbstractHermeticGUIControl {

  #region Initialization settings.
  readonly string caption;
  #endregion

  #region IRenderableGUIControl implementation
  /// <inheritdoc/>
  public override void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions) {
    GUI.changed = false;
    var value = GetMemberValue<bool>();
    value = GUILayout.Toggle(value, caption, GUI.skin.toggle);
    if (GUI.changed) {
      SetMemberValue(value);
    }
  }
  #endregion

  /// <summary>Creates the control.</summary>
  /// <param name="caption">The boolean control caption.</param>
  /// <param name="instance">The class instance that owns the field to control.</param>
  /// <param name="fieldInfo">The field information of thefield to control.</param>
  /// <param name="onUpdate">The callback to call when the value is changed.</param>
  /// <seealso cref="ConfigUtils.StandardOrdinaryTypesProto"/>
  public HermeticGUIControlBoolean(string caption, object instance, FieldInfo fieldInfo,
                                   Action onUpdate = null)
    : base(instance, fieldInfo, onUpdate) {
    this.caption = caption;
    if (GetMemberType() != typeof(bool)) {
      throw new ArgumentException(string.Format(
          "Unsupported type: type={0}", GetMemberType()));
    }
  }
}

}  // namespace
