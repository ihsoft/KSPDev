// Kerbal Development tools - Examples
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.GUIUtils;
using UnityEngine;

namespace Examples {

#region KeyboardEventTypeDemo1
public class KeyboardEventTypeDemo1 : PartModule {
  static readonly Message<KeyboardEventType> msg1 = new Message<KeyboardEventType>(
      "#TypeDemo_msg1", defaultTemplate: "Keybinding is [<<1>>]");

  // Depending on the current language in the system, this method will present different unit names. 
  void Show() {
    Debug.Log(msg1.Format(Event.KeyboardEvent("^1")));
    // Prints: Keybinding is [Ctrl+1]
    Debug.Log(msg1.Format(Event.KeyboardEvent("^[1]")));
    // Prints: Keybinding is [Ctrl+Alpha1]
    Debug.Log(msg1.Format(Event.KeyboardEvent("$^A")));
    // Prints: Keybinding is [Shift+Ctrl+A]
  }
}
#endregion

}  // namespace
