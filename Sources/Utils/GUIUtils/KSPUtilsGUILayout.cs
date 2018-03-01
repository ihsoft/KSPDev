// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Utility class to help building various GUILayout controls.</summary>
public static class KSPUtilsGUILayout {

  /// <summary>Makes a button that fires a callback when pressed or released.</summary>
  /// <remarks>
  /// The callabacks are only fired once for every state change, and are guarnteed to not be called
  /// during the layout phase.
  /// </remarks>
  /// <param name="btnState">The state to read and modify.</param>
  /// <param name="guiCnt">The GUI content to present as the button's caption.</param>
  /// <param name="style">The GUI style of the control.</param>
  /// <param name="options">The GUILayout options to apply to the control.</param>
  /// <param name="fnPush">The callback to call when the button is pressed.</param>
  /// <param name="fnRelease">The callback to call when the button is released.</param>
  public static void PushButton(
      ref bool btnState,
      GUIContent guiCnt,
      GUIStyle style,
      GUILayoutOption[] options,
      Action fnPush,
      Action fnRelease) {
    var state = GUILayout.RepeatButton(guiCnt, style, options);
    if (Event.current.type != EventType.Layout) {
      if (state && !btnState) {
        fnPush();
        btnState = true;
      } else if (!state && btnState) {
        fnRelease();
        btnState = false;
      }
    }
  }

  /// <summary>Makes a toggle control that fires a callback when teh state changes.</summary>
  /// <remarks>
  /// The callabacks are only fired once for every state change, and are guarnteed to not be called
  /// during the layout phase. The <c>GUI.changed</c> state is preserved by these control. I.e. if
  /// the button state has changed, then <c>GUI.changed</c> will be <c>true</c>. Otherwise, it will
  /// retain its value before entering the method.
  /// </remarks>
  /// <param name="btnState">The state to read and modify.</param>
  /// <param name="guiCnt">The GUI content to present as the button's caption.</param>
  /// <param name="style">The GUI style of the control.</param>
  /// <param name="options">The GUILayout options to apply to the control.</param>
  /// <param name="fnOn">The callback to call when the control is checked.</param>
  /// <param name="fnOff">The callback to call when the control is released.</param>
  public static void ToggleButton(
      ref bool btnState,
      GUIContent guiCnt,
      GUIStyle style,
      GUILayoutOption[] options,
      Action fnOn,
      Action fnOff) {
    var oldState = GUI.changed;
    GUI.changed = false;
    var state = GUILayout.Toggle(btnState, guiCnt, style, options);
    if (Event.current.type != EventType.Layout && GUI.changed) {
      if (btnState) {
        fnOn();
      } else {
        fnOff();
      }
      btnState = state;
    }
    GUI.changed &= oldState;
  }
}

}  // namespace
