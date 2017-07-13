// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to render big disabled bloacks of GUI.</summary>
/// <example><code source="Examples/GUIUtils/GuiColor-Examples.cs" region="GuiColorDemo1"/></example>
/// <example><code source="Examples/GUIUtils/GuiColor-Examples.cs" region="GuiColorDemo2"/></example>
public class GuiColor : IDisposable {
  readonly Color oldColor;
  readonly Color oldContentColor;
  readonly Color oldBackgroundColor;

  /// <summary>Stores the old state and sets a new one.</summary>
  /// <param name="color">The new color for <c>GUI.color</c>.</param>
  /// <param name="contentColor">The new color for <c>GUI.contentColor</c>.</param>
  /// <param name="backgroundColor">The new color for <c>GUI.backgroundColor</c>.</param>
  /// <example><code source="Examples/GUIUtils/GuiColor-Examples.cs" region="GuiColorDemo1"/></example>
  /// <example><code source="Examples/GUIUtils/GuiColor-Examples.cs" region="GuiColorDemo2"/></example>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.GUI.color']/*"/>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.GUI.contentColor']/*"/>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.GUI.backgroundColor']/*"/>
  public GuiColor(Color? color = null, Color? contentColor = null, Color? backgroundColor = null) {
    oldColor = GUI.color;
    if (color.HasValue) {
      GUI.color = color.Value;
    }
    oldContentColor = GUI.contentColor;
    if (contentColor.HasValue) {
      GUI.contentColor = contentColor.Value;
    }
    oldBackgroundColor = GUI.backgroundColor;
    if (backgroundColor.HasValue) {
      GUI.backgroundColor = backgroundColor.Value;
    }
  }

  /// <inheritdoc/>
  public void Dispose() {
    GUI.color = oldColor;
    GUI.contentColor = oldContentColor;
    GUI.backgroundColor = oldBackgroundColor;
  }
}

}  // namespace
