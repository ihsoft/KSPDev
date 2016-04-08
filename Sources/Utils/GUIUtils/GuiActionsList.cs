// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A helper to accumulate GUI actions.</summary>
/// <remarks>Unity may issue multiple GUI passes during a frame, and it requires number of UI
/// elements not to change between the passes. Unity expects the number of UI
/// controls in every pass to be exactly the same as in the very first one:
/// <see href="http://docs.unity3d.com/ScriptReference/EventType.Layout.html">EventType.Layout</see>.
/// When UI interactions affect representation all the changes must be postponed till the frame
/// rendering is ended. This helper can be used to store actions that will be executed at the
/// beginning of the next frame.</remarks>
/// <example>
/// <code>
/// public class MyUI : MonoBehaviour {
///   private readonly GuiActionsList guiActions = new GuiActionsList();
///   private bool showLabel = false;
///
///   void OnGUI() {
///     if (guiActions.ExecutePendingGuiActions()) {
///       // ...do other stuff that affects UI... 
///     }
///
///     if (GUILayout.Button(new GUIContent("Test Button"))) {
///       // If "showLabel" is changed right here then Unity GUI will complain saying the number
///       // of UI controls has changed. So, postpone the change until current frame is ended.
///       guiActions.Add(() => {
///         showLabel = !showLabel;  // This will be done at the beginning of the next frame.
///       });
///     }
///     
///     if (showLabel) {
///       GUILayout.Label("Test label");
///     }
///   }
/// }
/// </code>
/// <para>If you were using simple approach and updated <c>showLabel</c> right away Unity would
/// likely thrown an error like this:</para>
/// <para><c>[EXCEPTION] ArgumentException: Getting control 1's position in a group with only 1
/// controls when doing Repaint</c></para>
/// <seealso href="http://docs.unity3d.com/Manual/GUIScriptingGuide.html"/>
/// <seealso href="http://docs.unity3d.com/ScriptReference/EventType.html"/>
/// </example>
public class GuiActionsList {
  /// <summary>GUI action type.</summary>
  public delegate void GuiAction();
  
  /// <summary>A list of pending actions.</summary>
  private readonly List<GuiAction> guiActions = new List<GuiAction>();

  /// <summary>Adds an action to the pending list.</summary>
  /// <param name="actionFn">An action callback.</param>
  public void Add(GuiAction actionFn) {
    guiActions.Add(actionFn);
  }

  /// <summary>Executes actions when it's safe to do the changes.</summary>
  /// <remarks>It's safe to call this method in every pass. It will detect when it's safe to apply
  /// the changes and apply the changes only once per a frame.</remarks>
  /// <returns><c>true</c> if actions have been applied.</returns>
  public bool ExecutePendingGuiActions() {
    if (Event.current.type == EventType.Layout) {
      foreach (var actionFn in guiActions) {
        actionFn();
      }
      guiActions.Clear();
      return true;
    }
    return false;
  }
}

}  // namespace
