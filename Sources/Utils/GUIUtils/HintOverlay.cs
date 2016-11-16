// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>A wrapper class to present simple overlay window with text.</summary>
/// <remarks>
/// <para>Overlay windows doesn't have border or title. Main purpose of such windows is "hints".
/// I.e. short lived piece of information presented for the current context. The hint won't be shown
/// in UI until explicitly requsted via call to a <c>ShowAt*</c> method.</para>
/// <para>Keep in mind that this window contains graphics objects that will be destroyed on scene
/// re-loading. I.e. it must be re-created on every scene change.</para>
/// </remarks>
/// <example>
/// In a common case initialization of the hint window is done on the game object awakening, and
/// it's either shown or hidden in <c>OnGUI</c> method.
/// <code>
/// class MyMod : MonoBehaviour {
///   HintOverlay hint;
///
///   void Awake() {
///     hint = new HintOverlay(12, 3, Color.white, new Color(0f, 0f, 0f, 0.5f));
///   }
///
///   void OnGUI() {
///     hint.text = string.Format("Current frame is: {0}", Time.frameCount);
///     hint.ShowAtCursor();
///   }
/// }
/// </code>
/// <para>In the example above text of the hint is set on every frame update since frame count is updated
/// this frequently. Though, if your data is updated less frequently you may save some performance
/// by updating text in the methods different from <c>OnGUI</c>.</para>
/// </example>
public class HintOverlay {
  /// <summary>Padding when showing hint on the right side of the mouse cursor.</summary>
  public int RightSideMousePadding = 24;
  /// <summary>Padding when showing hint on the left side of the mouse cursor.</summary>
  public int LeftSideMousePadding = 4;

  /// <summary>The hint overlay text.</summary>
  /// <remarks>Linefeed symbols are correctly handled. Use them to make multiline content. Setting
  /// text is an expensive operation since it results in window size recalculation. Don't update it
  /// more frequently than the underlaying data does.</remarks>
  public string text {
    get { return _text; }
    set {
      _text = value;
      textSize = hintWindowStyle.CalcSize(new GUIContent(text)); 
    }
  }
  string _text;

  /// <summary>Size of the sample texture that fills hint window backgroud.</summary>
  /// <remarks>Small values may impact rendering performance. Large values will increase memory
  /// footpring. Choose it wise.</remarks>
  const int BackgroundTextureSize = 100;

  /// <summary>Precalculated UI text size for the currently assigned text.</summary>
  Vector2 textSize;
  /// <summary>Precalculated style for the hint overlay window.</summary>
  GUIStyle hintWindowStyle;

  /// <summary>Constructs an overaly.</summary>
  /// <param name="fontSize">Size of the text font in the hint.</param>
  /// <param name="padding">Padding between the text and the window boundaries.</param>
  /// <param name="textColor">Color of the hint text.</param>
  /// <param name="backgroundColor">Color of the hint background. If alpha component is different
  /// from <c>1.0</c> then background will be semi-transparent.</param>
  public HintOverlay(int fontSize, int padding, Color textColor, Color backgroundColor) {
    hintWindowStyle = new GUIStyle {
      normal = {
        background = CreateSampleTextureFromColor(backgroundColor),
        textColor = textColor
      },
      padding = new RectOffset(padding, padding, padding, padding),
      fontSize = fontSize
    };
  }

  /// <summary>Shows hint text at the current mouse pointer.</summary>
  /// <remarks>When possible the window is shown on the right side of the cursor. Though, if part of
  /// the window goes out of the screen then it will be shown on the left side. If bottom boundary
  /// of the window hits bottom boundary of the screen then hint is aligned vertically so what the
  /// full content is visible. </remarks>
  public void ShowAtCursor() {
    var xPos = Mouse.screenPos.x + RightSideMousePadding;
    if (xPos + textSize.x > Screen.width) {
      xPos = Mouse.screenPos.x - LeftSideMousePadding - textSize.x;
    }
    var yPos = Mouse.screenPos.y;
    if (yPos + textSize.y > Screen.height) {
      yPos = Screen.height - textSize.y;
    }
    ShowAtPosition(xPos, yPos);
  }

  /// <summary>Shows hint at the absolute screen position.</summary>
  /// <remarks>If hint content goes out of the screen it's clipped.</remarks>
  /// <param name="x">X position is screen coordinates.</param>
  /// <param name="y">Y position is screen coordinates.</param>
  public void ShowAtPosition(float x, float y) {
    var hintLabelRect = new Rect(x, y, textSize.x, textSize.y);
    GUI.Label(hintLabelRect, text, hintWindowStyle);
  }

  /// <summary>Creates a clear color texture to fill background with.</summary>
  /// <param name="color">A background color.</param>
  /// <returns>Texture of a default size.</returns>
  static Texture2D CreateSampleTextureFromColor(Color color) {
    var texture =
        new Texture2D(BackgroundTextureSize, BackgroundTextureSize, TextureFormat.ARGB32, false);
    texture.SetPixels(
        Enumerable.Repeat(color, BackgroundTextureSize * BackgroundTextureSize).ToArray());
    texture.Apply();
    texture.Compress(false /* highQuality */);
    return texture;
  }
}

}  // namespace
