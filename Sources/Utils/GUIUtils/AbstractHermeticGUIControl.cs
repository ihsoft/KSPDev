// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.LogUtils;
using System;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Base class for a GUI element that incapsulates a class instance member.</summary>
/// <remarks>
/// This kind of controls are used to represent and/or edit the values of various types, but not in
/// the usual <c>OnGUI</c> approach. The usual Unity approach assumes that the value is passed and
/// returned in each rendering call. However, for the hermetic control, the source of the value is
/// defined only once, at the creation time. Once it's done, the rendering is only called to have
/// the logic handled. That said, the caller rendering method doesn't care about the underlying
/// data, and that's why it's "hermetic".
/// <para>
/// The value source is either a field or a property of the class. Due to the need of using
/// reflections to access the member, these controls are not performance optimized. They are fine
/// for the debugging and settings dialogs, but for the performance demanding applications a regular
/// Unity approach is suggested.
/// </para>
/// </remarks>
public abstract class AbstractHermeticGUIControl : IRenderableGUIControl {

  #region Initialization settings.
  readonly object instance;
  readonly FieldInfo fieldInfo;
  readonly Action onUpdate;
  #endregion

  #region IRenderableGUIControl implementation
  /// <inheritdoc/>
  public abstract void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions);
  #endregion

  /// <summary>Creates a control, bound to a field.</summary>
  /// <param name="instance">The class instance that owns the field to control.</param>
  /// <param name="fieldInfo">The field information of thefield to control.</param>
  /// <param name="onUpdate">The callback to call when the value is changed.</param>
  protected AbstractHermeticGUIControl(object instance, FieldInfo fieldInfo,
                                       Action onUpdate = null) {
    this.instance = instance;
    this.fieldInfo = fieldInfo;
    this.onUpdate = onUpdate;
  }

  /// <summary>Returns the type of the member value.</summary>
  /// <returns></returns>
  protected Type GetMemberType() {
    return fieldInfo.FieldType;
  }

  /// <summary>Get the value from the controlled member.</summary>
  /// <returns>The value of the member.</returns>
  protected T GetMemberValue<T>() {
    return (T) fieldInfo.GetValue(instance);
  }

  /// <summary>Sets value to the controlled member.</summary>
  /// <param name="value">The new value to set.</param>
  /// <param name="actionsList">
  /// The actions list to submit the update actions into. If it's <c>null</c>, then the update and
  /// the notification will happen immediately.
  /// </param>
  protected void SetMemberValue<T>(T value, GuiActionsList actionsList = null) {
    if (actionsList != null) {
      actionsList.Add(() => {
        fieldInfo.SetValue(instance, value);
        UpdateMemberInstance();
      });
    } else {
      fieldInfo.SetValue(instance, value);
      UpdateMemberInstance();
    }
  }

  /// <summary>Calls the customized update method.</summary>
  /// <remarks>It's fail proof.</remarks>
  protected void UpdateMemberInstance() {
    if (onUpdate != null) {
      try {
        onUpdate();
      } catch (Exception ex) {
        DebugEx.Error("Exception in the update method: {0}", ex);
      }
    }
  }
}

}  // namespace
