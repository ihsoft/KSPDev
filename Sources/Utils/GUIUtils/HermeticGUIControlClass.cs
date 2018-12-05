// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ConfigUtils;
using KSPDev.DebugUtils;
using KSPDev.LogUtils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Hermetic GUI control for the class types.</summary>
/// <remarks>
/// The members in the class must be attributed with
/// <see cref="DebugUtils.DebugAdjustableAttribute"/> in order to be visible in the control.
/// </remarks>
/// <seealso cref="DebugUtils.DebugAdjustableAttribute"/>
/// <seealso cref="DebugUtils.DebugGui"/>
public sealed class HermeticGUIControlClass : AbstractHermeticGUIControl {

  #region Initialization settings.
  readonly string caption;
  readonly IRenderableGUIControl[] adjustableControls;
  #endregion

  /// <summary>Tells if the nested mebers should be presented.</summary>
  bool isExpanded;

  #region IRenderableGUIControl implementation
  /// <inheritdoc/>
  public override void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions) {
    using (new GUILayout.VerticalScope(GUI.skin.box)) {
      using (new GUILayout.HorizontalScope(GUIStyle.none)) {
        GUILayout.Label(caption);
        GUILayout.FlexibleSpace();
        var toggleCaption = isExpanded ? "\u25b2 Collapse Group" : "\u25bc Expand Group";
        if (GUILayout.Button(toggleCaption, layoutOptions)) {
          if (actionsList != null) {
            var isExpandedCopy = isExpanded;  // Lambda needs a copy!
            actionsList.Add(() => isExpanded = !isExpanded);
          } else {
            isExpanded = !isExpanded;
          }
        }
      }
      if (isExpanded) {
        foreach (var control in adjustableControls) {
          control.RenderControl(actionsList, layoutStyle, layoutOptions);
        }
      }
    }
  }
  #endregion

  /// <summary>Creates a control, bound to a member.</summary>
  /// <param name="caption">The boolean control caption.</param>
  /// <param name="instance">The class instance that owns the member to manage.</param>
  /// <param name="fieldInfo">The field to manage.</param>
  /// <param name="propertyInfo">The property to manage.</param>
  /// <param name="onUpdate">The callback to call when the value is changed.</param>
  public HermeticGUIControlClass(string caption, object instance,
                                 FieldInfo fieldInfo = null, PropertyInfo propertyInfo = null,
                                 Action onUpdate = null)
      : base(instance, fieldInfo, propertyInfo, onUpdate) {
    this.caption = caption;
    if (!GetMemberType().IsClass) {
      throw new ArgumentException("Unsupported type: " + GetMemberType());
    }
    var adjustables = new List<IRenderableGUIControl>();
    var obj = GetMemberValue<object>();
    adjustableControls = new List<DebugGui.DebugMemberInfo>()
        .Concat(DebugGui.GetAdjustableFields(obj))
        .Concat(DebugGui.GetAdjustableProperties(obj))
        .Concat(DebugGui.GetAdjustableActions(obj))
        .Select(m => new StdTypesDebugGuiControl(
            m.attr.caption, obj,
            host: instance as IHasDebugAdjustables,
            fieldInfo: m.fieldInfo, propertyInfo: m.propertyInfo, methodInfo: m.methodInfo)
        )
        .ToArray();
  }
}

}  // namespace
