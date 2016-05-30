// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev.GUIUtils {

/// <summary>Helper class to present global UI messages.</summary>
/// <remarks>The actual representation of the different priority messages depends on the KSP
/// version. As a rule of thumb use the following reasons when deciding how to show a message:
/// <list>
/// <item>Important messages should be show as "priority". It's assumed that UI layout is build so
///     what that such messages won't be missed.</item>
/// <item>Messages that only give status update and can be safely ignored by the player should be
///     reported as "info". They will show up in UI but not necessarily bring user's attention.
///     </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// class MyMod : MonoBehavior {
///   void Update() {
///     if (Input.GetKeyDown("1")) {
///       ScreenMessaging.ShowPriorityScreenMessage("Key pressed in frame #{0}", Time.frameCount);
///     }
///   }
/// }
/// </code>
/// </example>
public static class ScreenMessaging {
  const float DefaultMessageTimeout = 5f;  // Seconds.
  
  /// <summary>Shows a formatted message with the specified location and timeout.</summary>
  /// <param name="style">A <c>ScreenMessageStyle</c> specifier.</param>
  /// <param name="duration">Delay before hiding the message in seconds.</param>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  public static void ShowScreenMessage(
      ScreenMessageStyle style, float duration, String fmt, params object[] args) {
    ScreenMessages.PostScreenMessage(String.Format(fmt, args), duration, style);
  }

  /// <summary>Shows an important message with the specified timeout.</summary>
  /// <remarks>It's no defined how exactly the message is shown. The only thing required is that
  /// player won't miss it.</remarks>
  /// <param name="duration">Delay before hiding the message in seconds.</param>
  /// <param name="fmt">A formatting string.</param>
  /// <param name="args">Arguments for the formatting string.</param>
  public static void ShowPriorityScreenMessageWithTimeout(
      float duration, String fmt, params object[] args) {
    ShowScreenMessage(ScreenMessageStyle.UPPER_CENTER, duration, fmt, args);
  }

  /// <summary>Shows an important message with a default timeout.</summary>
  /// <remarks>It's no defined how exactly the message is shown. The only thing required is that
  /// player won't miss it.</remarks>
  /// <param name="fmt">A formatting string.</param>
  /// <param name="args">Arguments for the formatting string.</param>
  public static void ShowPriorityScreenMessage(String fmt, params object[] args) {
    ShowPriorityScreenMessageWithTimeout(DefaultMessageTimeout, fmt, args);
  }
      
  /// <summary>Shows an info message with the specified timeout.</summary>
  /// <remarks>It's no defined how exactly the message is shown.</remarks>
  /// <param name="duration">Delay before hiding the message in seconds.</param>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  public static void ShowInfoScreenMessageWithTimeout(
    float duration, String fmt, params object[] args) {
    ShowScreenMessage(ScreenMessageStyle.UPPER_RIGHT, duration, fmt, args);
  }

  /// <summary>Shows an info message with a default timeout.</summary>
  /// <remarks>It's no defined how exactly the message is shown.</remarks>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  public static void ShowInfoScreenMessage(String fmt, params object[] args) {
    ShowInfoScreenMessageWithTimeout(DefaultMessageTimeout, fmt, args);
  }
}

}  // namespace
