// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Control for an auto-resizing scrolling control.</summary>
/// <remarks>
/// This control automatically shrinks to its content to take as small spacen as possible, while
/// still not needing the scrolling. If the height of the content exceeds the specified maximum,
/// then a vertical scrollbar is added to the view and the view doesn't expand further.
/// </remarks>
public sealed class GUILayoutVerticalScrollView {
  /// <summary>The scrolling piosition of the area.</summary>
  public Vector2 scrollPosition = Vector2.zero;

  /// <summary>The full height of the area.</summary>
  public float scrollableAreaHeight { get; private set; }

  /// <summary>Starts the scrollable view.</summary>
  /// <param name="layoutStyle">The style of the scrollable area.</param>
  /// <param name="maxHeight">
  /// The maximum height, the control is allowed to grow to before switching to the scrolling.
  /// </param>
  /// <param name="layoutOptions">
  /// The layout options for the view. Don't set the height options! This dimension is controlled by
  /// the control.
  /// </param>
  public void BeginView(GUIStyle layoutStyle, float maxHeight,
                        params GUILayoutOption[] layoutOptions) {
    GUILayoutOption[] options;
    if (layoutOptions.Length > 0) {
      options = layoutOptions
          .Concat(new[] {
              GUILayout.MinHeight(scrollableAreaHeight),
              GUILayout.MaxHeight(maxHeight),
          })
          .ToArray();
    } else {
      options = new[] {
          GUILayout.MinHeight(scrollableAreaHeight),
          GUILayout.MaxHeight(maxHeight),
      };
    }
    if (scrollableAreaHeight < maxHeight) {
      // Don't show the bars when estimating the size of the content.
      scrollPosition = GUILayout.BeginScrollView(
          scrollPosition, false, false, GUIStyle.none, GUIStyle.none, layoutStyle, options);
    } else {
      scrollPosition = GUILayout.BeginScrollView(scrollPosition, layoutStyle, options);
    }
  }

  /// <summary>Marks the scrollable view end.</summary>
  public void EndView() {
    // A hacky way to get the actual height of the scrollable area.
    GUILayout.Label(GUIContent.none, GUIStyle.none, GUILayout.MaxHeight(0));
    if (Event.current.type == EventType.repaint) {
      scrollableAreaHeight = GUILayoutUtility.GetLastRect().y;
    }
    GUILayout.EndScrollView();
  }
}

}  // namespace
