// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSPDev.GUIUtils {
  
/// <summary>Flags to specify key modifiers combination.</summary>
[FlagsAttribute]
public enum KeyModifiers {
  /// <summary>No modifier keys are pressed.</summary>
  /// <remarks>This value only makes sense when used alone. Combining it with any other value
  /// will result in ignoring value <c>None</c>.</remarks>
  None = 0,
  /// <summary>Left or right ALT key pressed.</summary>
  AnyAlt = 0x01,
  /// <summary>Left or right SHIFT key pressed.</summary>
  AnyShift = 0x02,
  /// <summary>Left or right CONTROL key pressed.</summary>
  AnyControl = 0x04          
}

/// <summary>A helper to verify various event handling conditions.</summary>
public static class EventChecker {
  /// <summary>Verifies that the requested key modifiers are pressed.</summary>
  /// <remarks>The check will succeed only if the exact set of modifier keys is pressed. If there
  /// are more or less modifiers pressed the check will fail. E.g. if there are <c>LeftAlt</c> and
  /// <c>LeftShift</c> pressed but the check is executed against
  /// <c>AnyShift</c> then it will fail. Though, checking for <c>AnyShift | AnyAlt</c> will succeed.
  /// <para>In case of checking for <c>None</c> the check will require no modifier keys to be
  /// pressed. If you deal with mouse button events it's a good idea to verify if no modifiers are
  /// pressed even if you don't care about other combinations. It will let other modders to use
  /// mouse buttons and not to interfere with your mod.</para>
  /// </remarks>
  /// <param name="modifiers">A combination of key modifiers to verify.</param>
  /// <returns><c>true</c> when exactly the requested combination is pressed.</returns>
  public static bool IsModifierCombinationPressed(KeyModifiers modifiers) {
    bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    return !((shiftPressed ^ (modifiers & KeyModifiers.AnyShift) == KeyModifiers.AnyShift)
        | (altPressed ^ (modifiers & KeyModifiers.AnyAlt) == KeyModifiers.AnyAlt)
        | (ctrlPressed ^ (modifiers & KeyModifiers.AnyControl) == KeyModifiers.AnyControl));
  }
}
 
} // namespace

