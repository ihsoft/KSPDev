// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.Extensions;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A utility class to deal with the GUI windows.</summary>
/// <example><code source="Examples/GUIUtils/GuiWindow-Examples.cs" region="GuiWindowDemo1"/></example>
public static class GuiWindow {
  /// <summary>Latest mouse position to which the dragged window position has updated.</summary>
  /// <remarks>When this field is <c>null</c>, it means no window is being dragged.</remarks>
  static Vector2? dragPosition;

  /// <summary>
  /// Makes the window movable. It's an improved version of the stock <c>GUI.DragWindow()</c>
  /// method.
  /// </summary>
  /// <remarks>
  /// The stock method cancels dragging when the window layout is changed. It makes it useless when
  /// dealing with windows that can change their layout depending on the position. This method
  /// doesn't have this drawback. Moreover, it can tell if the window is being dragged, so that the
  /// code could postpone the layout update until the dragging is over. 
  /// </remarks>
  /// <param name="windowRect">
  /// The window rectangle. It must be the same instance which is passed to the
  /// <c>GUILayout.Window</c> method.
  /// </param>
  /// <param name="dragArea">
  /// The rectangle in the local windows's space that defines the dragging area. In case of it's
  /// out of bounds of the window rectangle, it will be clipped.
  /// </param>
  /// <returns><c>true</c> if the window is being dragged.</returns>
  /// <example><code source="Examples/GUIUtils/GuiWindow-Examples.cs" region="GuiWindowDemo1"/></example>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.GUI.DragWindow']/*"/>
  public static bool DragWindow(ref Rect windowRect, Rect dragArea) {
    var mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    if (dragPosition.HasValue) {
      windowRect.position += mousePosition - dragPosition.Value;
      dragPosition = mousePosition;
    }
    var mouseEvent = Event.current;
    if (mouseEvent.isMouse && mouseEvent.button == 0) {
      if (mouseEvent.type == EventType.MouseDown) {
        dragArea.position += windowRect.position;
        dragArea = dragArea.Intersect(windowRect);
        if (dragArea.Contains(mousePosition)) {
          dragPosition = mousePosition;
        }
      } else if (mouseEvent.type == EventType.MouseUp) {
        if (dragPosition.HasValue) {
          dragPosition = null;
        }
      }
    }
    return dragPosition.HasValue;
  }
}

}  // namespace
