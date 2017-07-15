// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region GuiWindowDemo1
public class GuiWindowDemo1 : MonoBehaviour {
  Rect windowRect;
  Rect titleBarRect = new Rect(0, 0, 10000, 20);

  void OnGUI() {
    windowRect = GUILayout.Window(12345, windowRect, WindowFunc, "Test title");
  }

  void WindowFunc(int windowId) {
    // ...add the controls....

    // Allow the window to be dragged by its title bar.
    GuiWindow.DragWindow(ref windowRect, titleBarRect);
  }
}
#endregion

}  // namespace
