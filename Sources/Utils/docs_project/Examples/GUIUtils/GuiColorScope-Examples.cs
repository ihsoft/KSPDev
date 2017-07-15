// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region GuiColorDemo1
public class GuiColorDemo1 : MonoBehaviour {
  void OnGUI() {
    // Set any color settings.
    GUI.contentColor = Color.white;
    GUI.backgroundColor = Color.black;
    GUILayout.Button("B&W button");

    // Make any changes to the colors in this block.
    using (new GuiColorScope()) {
      GUI.color = Color.red;
      GUILayout.Button("Red button");
      GUI.contentColor = Color.green;
      GUI.backgroundColor = Color.blue;
      GUILayout.Button("Green on Blue button");
    }

    // From here all the colors will be restored back.
    GUILayout.Button("B&W button");
  }
}
#endregion

#region GuiColorDemo2
public class GuiColorDemo2 : MonoBehaviour {
  void OnGUI() {
    // Set any color settings.
    GUI.contentColor = Color.white;
    GUI.backgroundColor = Color.black;
    GUILayout.Button("B&W button");

    using (new GuiColorScope(color: Color.red)) {
      GUILayout.Button("Red button");
    }
    using (new GuiColorScope(backgroundColor: Color.red)) {
      GUILayout.Button("White on Red button");
    }
    using (new GuiColorScope(contentColor: Color.red)) {
      GUILayout.Button("red on balck button");
    }

    GUILayout.Button("B&W button");
  }
}
#endregion

}  // namespace
