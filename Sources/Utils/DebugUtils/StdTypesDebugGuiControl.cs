// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using KSPDev.GUIUtils;
using KSPDev.LogUtils;
using System;
using System.Reflection;
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

  readonly Action action;

  /// <summary>Creates a debug adjustment control for the basic type.</summary>
  /// <param name="caption">The field caption to show in the dialog.</param>
  /// <param name="instance">
  /// The instance of the object that holds the field to be adjusted via GUI.
  /// </param>
  /// <param name="host">
  /// The class that should get the member change notifications. If not set, then
  /// <paramref name="instance"/> will be the target.
  /// </param>
  /// <param name="fieldInfo">The field info of the target member.</param>
  /// <param name="propertyInfo">The property info of the target member.</param>
  /// <param name="methodInfo">The action member info.</param>
  public StdTypesDebugGuiControl(string caption, object instance,
                                 IHasDebugAdjustables host = null,
                                 FieldInfo fieldInfo = null,
                                 PropertyInfo propertyInfo = null,
                                 MethodInfo methodInfo = null) {
    Action onValueUpdatedCallback = null;
    var adjustable = (host as IHasDebugAdjustables)
        ?? (instance as IHasDebugAdjustables);
    if (adjustable != null) {
      onValueUpdatedCallback = adjustable.OnDebugAdjustablesUpdated;
    }
    try {
      if (methodInfo != null) {
        if (methodInfo.GetParameters().Length > 0) {
          throw new ArgumentException("Debug action method must be parameterless");
        }
        this.caption = caption;
        this.action = () => methodInfo.Invoke(instance, new object[0]);
      } else {
        this.caption = caption + ":";
        this.action = null;
        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
        if (type == typeof(bool)) {
          this.control = new HermeticGUIControlBoolean(
              caption, instance,
              fieldInfo: fieldInfo, propertyInfo: propertyInfo, onUpdate: onValueUpdatedCallback);
        } else if (type.IsEnum) {
          this.control = new HermeticGUIControlSwitch(
              instance,
              fieldInfo: fieldInfo, propertyInfo: propertyInfo, onUpdate: onValueUpdatedCallback,
              useOwnLayout: false);
        } else {
          var proto = new StandardOrdinaryTypesProto();
          if (proto.CanHandle(type)) {
            this.control = new HermeticGUIControlText(
                instance,
                fieldInfo: fieldInfo, propertyInfo: propertyInfo, onUpdate: onValueUpdatedCallback,
                useOwnLayout: false);
          } else {
            this.control = new HermeticGUIControlClass(
                caption, instance,
                fieldInfo: fieldInfo, propertyInfo: propertyInfo,
                onUpdate: onValueUpdatedCallback);
          }
        }
      }
    } catch (Exception ex) {
      if (fieldInfo != null) {
        DebugEx.Error(
            "Failed to bind to field {0}.{1} => {2}: {3}",
            fieldInfo.DeclaringType.FullName, fieldInfo.Name, fieldInfo.FieldType, ex);
      } else if (propertyInfo != null) {
        DebugEx.Error(
            "Failed to bind to property {0}.{1} => {2}: {3}",
            propertyInfo.DeclaringType.FullName, propertyInfo.Name, propertyInfo.PropertyType, ex);
      } else {
        DebugEx.Error(
            "Failed to bind to method {0}.{1} => {2}: {3}",
            methodInfo.DeclaringType.FullName, methodInfo.Name, methodInfo.ReturnType, ex);
      }
    }
  }

  /// <inheritdoc/>
  public void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] options) {
    if (control != null) {
      if (control is HermeticGUIControlClass || control is HermeticGUIControlBoolean) {
        control.RenderControl(actionsList, layoutStyle, options);
      } else {
        using (new GUILayout.HorizontalScope(GUI.skin.box)) {
          GUILayout.Label(caption);
          GUILayout.FlexibleSpace();
          control.RenderControl(actionsList, layoutStyle, options);
        }
      }
    } else if (action != null) {
      if (GUILayout.Button(caption)) {
        if (actionsList != null) {
          actionsList.Add(action);
        } else {
          action();
        }
      }
    }
  }
}

}  // namespace
