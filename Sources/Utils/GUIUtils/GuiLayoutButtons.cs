// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Utility class to help building GUI layout buttons.</summary>
public static class GUILayoutButtons {
  /// <summary>Makes a button that fires a callback when pressed or released.</summary>
  /// <remarks>
  /// The callbacks are only fired once for every state change, and are guarnteed to not be called
  /// during the layout phase. The callback is called *after* the state is updated. 
  /// </remarks>
  /// <param name="btnState">The current press state of the button.</param>
  /// <param name="guiCnt">The GUI content to present as the button's caption.</param>
  /// <param name="style">The GUI style of the control.</param>
  /// <param name="options">
  /// The GUILayout options to apply to the control. It can be <c>null</c>.
  /// </param>
  /// <param name="fnPush">
  /// The callback to call when the button is pressed. Can be <c>null</c>.
  /// </param>
  /// <param name="fnRelease">
  /// The callback to call when the button is released. Can be <c>null</c>.
  /// </param>
  /// <param name="actionsList">
  /// The action list to execute the callbacks thru. If the list is omitted, then the callback is
  /// fired right away. Otherwise, it's added to the list and executed when the list is triggered.
  /// </param>
  /// <returns>The new button press state.</returns>
  /// <seealso cref="GuiActionsList"/>
  public static bool Push(bool btnState, GUIContent guiCnt, GUIStyle style,
                          GUILayoutOption[] options, Action fnPush, Action fnRelease,
                          GuiActionsList actionsList = null) {
    var state = GUILayout.RepeatButton(guiCnt, style, options);
    if (Event.current.type != EventType.Layout) {
      if (!GUI.enabled) {
        state = false;
      }
      if (state != btnState) {
        btnState = state;
        if (btnState) {
          if (fnPush != null) {
            if (actionsList == null) {
              fnPush();
            } else {
              actionsList.Add(fnPush);
            }
          }
        } else {
          if (fnRelease != null) {
            if (actionsList == null) {
              fnRelease();
            } else {
              actionsList.Add(fnRelease);
            }
          }
        }
      }
    }
    return btnState;
  }

  /// <summary>Makes a toggle control that fires a callback when the state changes.</summary>
  /// <remarks>
  /// The callbacks are only fired once for every state change, and are guaranteed to not be called
  /// during the layout phase. The callback is called *after* the state is updated.
  /// </remarks>
  /// <param name="btnState">The current toggle state.</param>
  /// <param name="guiCnt">The GUI content to present as the button's caption.</param>
  /// <param name="style">The GUI style of the control.</param>
  /// <param name="options">
  /// The GUILayout options to apply to the control. It can be <c>null</c>.
  /// </param>
  /// <param name="fnOn">
  /// The callback to call when the control is checked. Can be <c>null</c>.
  /// </param>
  /// <param name="fnOff">The callback to call when the control is checked. Can be <c>null</c>.
  /// </param>
  /// <param name="actionsList">
  /// The action list to execute the callbacks thru. If the list is omitted, then the callback is
  /// fired right away. Otherwise, it's added to the list and executed when the list is triggered.
  /// </param>
  /// <returns>The new button toggle state.</returns>
  /// <seealso cref="GuiActionsList"/>
  public static bool Toggle(bool btnState, GUIContent guiCnt, GUIStyle style,
                            GUILayoutOption[] options, Action fnOn, Action fnOff,
                            GuiActionsList actionsList = null) {
    GUI.changed = false;
    var state = GUILayout.Toggle(btnState, guiCnt, style, options);
    if (Event.current.type != EventType.Layout) {
      if (!GUI.enabled) {
        state = false;
      }
      if (btnState != state) {
        btnState = state;
        if (btnState) {
          if (fnOn != null) {
            if (actionsList == null) {
              fnOn();
            } else {
              actionsList.Add(fnOn);
            }
          }
        } else {
          if (fnOff != null) {
            if (actionsList == null) {
              fnOff();
            } else {
              actionsList.Add(fnOff);
            }
          }
        }
      }
    }
    return btnState;
  }
}

}  // namespace
