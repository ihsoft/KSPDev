// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;  // For the XML docs.

namespace KSPDev.GUIUtils {

/// <summary>Generic interface for the modules that implement a GUI events handling.</summary>
public interface IHasGUI {
  /// <summary>A callback which is called for every GUI event.</summary>
  /// <remarks>
  /// <para>
  /// On the every frame update Unity sends at least two events to every handler:
  /// <see cref="EventType.Layout"/> and <see cref="EventType.Repaint"/>. There can be more input
  /// events sent. The implementation must not change the number of controls in the view after the
  /// layout event.
  /// </para>
  /// <para>
  /// The modules don't need to implement this intefrace to be notified. It's called by the Unity
  /// core via messanging. However, implementing the interface make the inheritance and the overall
  /// code maintainability better.
  /// </para>
  /// </remarks>
  void OnGUI();
}

}  // namespace
