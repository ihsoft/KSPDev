// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using UnityEngine;

namespace KSPDev.GUIUtils {

/// <summary>Utility class to draw a simple table with the text colum contents.</summary>
/// <remarks>
/// <para>
/// This table cannot hold non-string content. It keeps all the columns to be of the same, and the
/// size is adjusted to the maximum column's size in the rows. There is a one frame delay between
/// the content change and the column resizing, which may result in flickering if the content
/// changes too frequently. The columns try to take as small space as possible, so defining the
/// minimum size may be a good bet.
/// </para>
/// <para>
/// This class is designed to be called on every frame. It's heavily performance optimized.
/// </para>
/// </remarks>
public class GUILayoutStringTable {
  /// <summary>Index of the curently rendered column.</summary>
  int currentIndex;

  /// <summary>Current frame maximum widths of the columns.</summary>
  float[] columnWidths;

  /// <summary>The maximum widths of the columns from the previous frame.</summary>
  float[] lastFrameColumnWidths;

  /// <summary>Creates a table of the specified column width.</summary>
  /// <remarks>
  /// It's OK to render more columns than reserved. They won't resized, but it's not an error.
  /// </remarks>
  /// <param name="columns">The number of columns to track.</param>
  public GUILayoutStringTable(int columns) {
    columnWidths = new float[columns];
    lastFrameColumnWidths = new float[columns];
  }

  /// <summary>Updates the table state each frame to remember the best column size values.</summary>
  /// <remarks>
  /// This method is only interested in the <c>EventType.Layout</c> phase, so no need to call it on
  /// each GUI event when there is a cheap way to detect it.
  /// </remarks>
  public void UpdateFrame() {
    if (Event.current.type == EventType.Layout) {
      lastFrameColumnWidths = columnWidths;
      columnWidths = new float[lastFrameColumnWidths.Length];
    }
  }

  /// <summary>Tells that a new row is about to be rendered.</summary>
  /// <remarks>Call it before every new row.</remarks>
  public void StartNewRow() {
    currentIndex = 0;
  }

  /// <summary>Adds a text column into the table.</summary>
  /// <param name="text">The text to add.</param>
  /// <param name="style">
  /// The style to apply to the text. If not set, then <c>GUI.skin.label</c> is used.
  /// </param>
  public void AddTextColumn(string text, GUIStyle style = null) {
    AddTextColumn(new GUIContent(text), style ?? GUI.skin.label);
  }

  /// <summary>Adds a text column into the table.</summary>
  /// <param name="text">The text to add.</param>
  /// <param name="message">The localizable message to get the minimum size from.</param>
  /// <param name="style">
  /// The style to apply to the text. If not set, then <c>GUI.skin.label</c> is used.
  /// </param>
  /// <seealso cref="LocalizableMessage"/>
  public void AddTextColumn(string text, LocalizableMessage message, GUIStyle style = null) {
    AddTextColumn(new GUIContent(text), style ?? GUI.skin.label,
                  minWidth: message.guiTags.minWidth, maxWidth: message.guiTags.maxWidth);
  }

  /// <summary>Adds a text column into the table with a value tooltip.</summary>
  /// <param name="text">The text to add.</param>
  /// <param name="tooltip">
  /// The tooltip for the column value. Note, that the tooltip is not handled by the table, it gets
  /// rendered by the Unity GUI functionality, which may need to be configured.
  /// </param>
  /// <param name="style">
  /// The style to apply to the text. If not set, then <c>GUI.skin.label</c> is used.
  /// </param>
  public void AddTextColumn(string text, string tooltip, GUIStyle style = null) {
    AddTextColumn(new GUIContent(text, tooltip), style ?? GUI.skin.label);
  }

  /// <summary>Adds a text column into the table with a value tooltip.</summary>
  /// <param name="text">The text to add.</param>
  /// <param name="tooltip">
  /// The tooltip for the column value. Note, that the tooltip is not handled by the table, it gets
  /// rendered by the Unity GUI functionality, which may need to be configured.
  /// </param>
  /// <param name="message">The localizable message to get the minimum size from.</param>
  /// <param name="style">
  /// The style to apply to the text. If not set, then <c>GUI.skin.label</c> is used.
  /// </param>
  /// <seealso cref="LocalizableMessage"/>
  public void AddTextColumn(string text, string tooltip, LocalizableMessage message,
                            GUIStyle style = null) {
    AddTextColumn(new GUIContent(text, tooltip), style ?? GUI.skin.label,
                  minWidth: message.guiTags.minWidth, maxWidth: message.guiTags.maxWidth);
  }

  /// <summary>Adds a content into the table column.</summary>
  /// <remarks>
  /// When possible, this method should be preferred over the other methods, which are simply the
  /// shortcuts to this one.</remarks>
  /// <param name="content">The text/tooltip content of the column to add.</param>
  /// <param name="style">The style to apply to the text.</param>
  /// <param name="minWidth">The minimum width of the column.</param>
  /// <param name="maxWidth">The maximum width of the column.</param>
  public void AddTextColumn(GUIContent content, GUIStyle style,
                            float minWidth = 0, float maxWidth = float.PositiveInfinity) {
    if (currentIndex >= columnWidths.Length) {
      // This column was not planned by the caller, so simply pass it through.
      GUILayout.Label(content, style);
      return;
    }
    if (Event.current.type == EventType.Layout) {
      // In the layout phase only calculate the size. Don't limit or resize the width of the area. 
      var size = style.CalcSize(content);
      var width = Mathf.Min(Mathf.Max(size.x, minWidth), maxWidth);
      if (width > columnWidths[currentIndex]) {
        columnWidths[currentIndex] = width;
      }
    }
    GUILayout.Label(content, style, GUILayout.Width(lastFrameColumnWidths[currentIndex]));
    currentIndex++;
  }
}

}  // namespace
