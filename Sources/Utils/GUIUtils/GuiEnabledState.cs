// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to render big disabled bloacks of GUI.</summary>
/// <example><code source="Examples/GUIUtils/GuiWindow-Examples.cs" region="GuiEnabledStateDemo1_OnGUI"/></example>
public class GuiEnabledState : IDisposable {
  readonly bool oldState;

  /// <summary>Stores the old state and sets a new one.</summary>
  /// <param name="newState">The new state to set.</param>
  /// <example><code source="Examples/GUIUtils/GuiWindow-Examples.cs" region="GuiEnabledStateDemo1_OnGUI"/></example>
  public GuiEnabledState(bool newState) {
    oldState = GUI.enabled;
    GUI.enabled = newState;
  }

  /// <inheritdoc/>
  public void Dispose() {
    GUI.enabled = oldState;
  }
}

}  // namespace
