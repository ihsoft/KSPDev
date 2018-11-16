// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Reflection;
using KSPDev.GUIUtils;
using KSPDev.LogUtils;
using UnityEngine;

namespace KSPDev.DebugUtils {

/// <summary>Wrapper class that gives a GUI control for editing the standard game types.</summary>
/// <remarks>
/// This control is <i>not</i> performance optimized. It's primarily designed for the simplicity of
/// the calling code in the cases like a simple settings dialog or a debug inspector.
/// </remarks>
/// <seealso cref="DebugAdjustableAttribute"/>
/// <seealso cref="AbstractHermeticGUIControl"/>
/// <seealso cref="IHasDebugAdjustables"/>
public sealed class StdTypesDebugGuiControl : IRenderableGUIControl {

  /// <summary>Human friendly string to present to label the value.</summary>
  readonly string caption;

  /// <summary>The actual control that handles the value.</summary>
  readonly IRenderableGUIControl control;

  /// <summary>Creates a debug adjustment control for the basic type.</summary>
  /// <param name="caption">The field caption to show in the dialog.</param>
  /// <param name="instance">
  /// The instance of the object that holds the field to be adjusted via GUI.
  /// </param>
  /// <param name="fieldInfo">The field info the target.</param>
  public StdTypesDebugGuiControl(string caption, object instance, FieldInfo fieldInfo) {
    this.caption = caption + ":";
    try {
      Action onValueUpdatedCallback = null;
      var adjustable = instance as IHasDebugAdjustables;
      if (adjustable != null) {
        onValueUpdatedCallback = adjustable.OnDebugAdjustablesUpdated;
      }
      if (fieldInfo.FieldType == typeof(bool)) {
        this.control = new HermeticGUIControlBoolean(
            caption, instance, fieldInfo, onUpdate: onValueUpdatedCallback);
      } else if (fieldInfo.FieldType.IsEnum) {
        this.control = new HermeticGUIControlSwitch(
            instance, fieldInfo, onUpdate: onValueUpdatedCallback, useOwnLayout: false);
      } else {
        this.control = new HermeticGUIControlText(
            instance, fieldInfo, onUpdate: onValueUpdatedCallback, useOwnLayout: false);
      }
    } catch (Exception ex) {
      DebugEx.Error("Failed to bind to {0}.{1} => {2}: {3}",
                    fieldInfo.DeclaringType.FullName, fieldInfo.Name, fieldInfo.FieldType, ex);
      this.control = null;
    }
  }

  /// <inheritdoc/>
  public void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] options) {
    if (control != null) {
      if (control is HermeticGUIControlBoolean) {
        control.RenderControl(actionsList, layoutStyle, options);
      } else {
        using (new GUILayout.HorizontalScope(GUI.skin.box)) {
          GUILayout.Label(caption);
          GUILayout.FlexibleSpace();
          control.RenderControl(actionsList, layoutStyle, options);
        }
      }
    }
  }
}

}  // namespace
