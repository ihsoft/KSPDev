// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

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
/// class MyMod : MonoBehaviour {
///   void Update() {
///     if (Input.GetKeyDown("1")) {
///       ScreenMessaging.ShowPriorityScreenMessage("Key pressed in frame #{0}", Time.frameCount);
///     }
///   }
/// }
/// </code>
/// </example>
public static class ScreenMessaging {
  /// <summary>Default timeout for a regular UI message.</summary>
  public const float DefaultMessageTimeout = 5f;  // Seconds.
  /// <summary>Default timeout for a UI message that reports an error.</summary>
  public const float DefaultErrorTimeout = 10f;  // Seconds.
  /// <summary>Default timeout for a UI message that warns about unusual conditions.</summary>
  public const float DefaultWarningTimeout = 5f;  // Seconds.
  /// <summary>Default color for the error messages.</summary>
  public readonly static Color ErrorColor = new Color(1f, 0.2f, 0.2f);
  /// <summary>Default color for the warning messages.</summary>
  public readonly static Color WarningColor = new Color(1f, 1, 0.2f);

  /// <summary>Wraps string into Unity rich-text tags to set a color.</summary>
  /// <remarks>Note, that Unity rich-text is not the same as RTF specification.</remarks>
  /// <param name="str">String to wrap.</param>
  /// <param name="color">Color to apply to the string.</param>
  /// <returns>Unity rich-text string.</returns>
  /// <seealso href="https://docs.unity3d.com/Manual/StyledText.html">
  /// Unity 3D: Rich-Text styled text</seealso>
  public static string SetColorToRichText(string str, Color color) {
    return String.Format(
        "<color=#{0:x02}{1:x02}{2:x02}{3:x02}>{4}</color>",
        Mathf.RoundToInt(255f * color.r),
        Mathf.RoundToInt(255f * color.g),
        Mathf.RoundToInt(255f * color.b),
        Mathf.RoundToInt(255f * color.a),
        str);
  }

  /// <summary>Shows a formatted message with the specified location and timeout.</summary>
  /// <param name="style">A <c>ScreenMessageStyle</c> specifier.</param>
  /// <param name="duration">Delay before hiding the message in seconds.</param>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  /// <seealso href="https://kerbalspaceprogram.com/api/class_screen_messages.html">
  /// KSP: ScreenMessages</seealso>
  /// <seealso href="https://kerbalspaceprogram.com/api/_screen_messages_8cs.html#ac19a4c3800d327889475848ccbbf9317">
  /// KSP: ScreenMessageStyle</seealso>
  public static void ShowScreenMessage(
      ScreenMessageStyle style, float duration, String fmt, params object[] args) {
    ScreenMessages.PostScreenMessage(String.Format(fmt, args), duration, style);
  }

  /// <summary>Shows a formatted message with the specified location and timeout.</summary>
  /// <param name="style"><see cref="ScreenMessageStyle"/> specifier.</param>
  /// <param name="duration">Delay before hiding the message in seconds.</param>
  /// <param name="color">Color to apply on the string.</param>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  /// <seealso href="https://kerbalspaceprogram.com/api/class_screen_messages.html">
  /// KSP: ScreenMessages</seealso>
  /// <seealso href="https://kerbalspaceprogram.com/api/_screen_messages_8cs.html#ac19a4c3800d327889475848ccbbf9317">
  /// KSP: ScreenMessageStyle</seealso>
  public static void ShowScreenMessage(
      ScreenMessageStyle style, float duration, Color color, String fmt, params object[] args) {
    ScreenMessages.PostScreenMessage(
        SetColorToRichText(String.Format(fmt, args), color), duration, style);
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

  /// <summary>Shows an error message with a default timeout.</summary>
  /// <remarks>
  /// It's not defined how exactly the message is shown, but it's guaranteed that it will look like
  /// an "error", and the player will perceive it like that.
  /// </remarks>
  /// <param name="fmt">The <c>string.Format()</c> formatting string.</param>
  /// <param name="args">The arguments for the formattign string.</param>
  public static void ShowErrorScreenMessage(String fmt, params object[] args) {
    ScreenMessages.PostScreenMessage(
        SetColorToRichText(String.Format(fmt, args), ErrorColor),
        DefaultErrorTimeout, ScreenMessageStyle.UPPER_RIGHT);
  }

  /// <summary>Shows a warning message with a default timeout.</summary>
  /// <remarks>
  /// It's no defined how exactly the message is shown, but it's guaranteed it looks like an error
  /// and player will perceive it like that.
  /// </remarks>
  /// <param name="fmt"><c>string.Format()</c> formatting string.</param>
  /// <param name="args">Arguments for the formattign string.</param>
  public static void ShowWarningScreenMessage(String fmt, params object[] args) {
    ScreenMessages.PostScreenMessage(
        SetColorToRichText(String.Format(fmt, args), WarningColor),
        DefaultWarningTimeout, ScreenMessageStyle.UPPER_RIGHT);
  }
}

}  // namespace
