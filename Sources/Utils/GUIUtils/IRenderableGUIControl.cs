// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Interface for a generic GUI control that can manage it's state.</summary>
/// <remarks>
/// For such controls it must be enough to only call the rendering method to have them properly
/// displaying and updating the underlying state.
/// </remarks>
public interface IRenderableGUIControl {
  /// <summary>Renders the control in GUI and handles all the interactions.</summary>
  /// <param name="actionsList">
  /// The actions list to submit the update operations into. It can be <c>null</c>, in which case
  /// the actions are expected to be executed right away.
  /// </param>
  /// <param name="layoutStyle">The style for the control canvas.</param>
  /// <param name="layoutOptions">The options for the controls canvas.</param>
  void RenderControl(
      GuiActionsList actionsList, GUIStyle layoutStyle, GUILayoutOption[] layoutOptions);
}

}  // namespace
