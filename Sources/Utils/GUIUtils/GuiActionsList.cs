// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A helper to accumulate GUI actions.</summary>
/// <remarks>Unity may issue multiple GUI passes during a frame, and it requires number of UI
/// elements not to change between the passes. When UI selection affects representation all the
/// changes must be postponed till it's safe to do the changes. This helper can be used to store
/// actions that will be executed at the very last pass.</remarks>
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
  /// <returns><c>true</c> if actuions have been applied.</returns>
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
