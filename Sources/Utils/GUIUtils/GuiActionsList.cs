// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>
/// </summary>
// FIXME: give descriptions.
public class GuiActionsList {
  public delegate void GuiAction();
  
  private readonly List<GuiAction> guiActions = new List<GuiAction>();
  
  public void Add(GuiAction actionFn) {
    guiActions.Add(actionFn);
  }
  
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
