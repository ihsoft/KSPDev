// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.Extensions {

/// <summary>Helper extensions to handel Unity rectangles.</summary>
/// <example><code source="Examples/Extensions/RectExtensions-Examples.cs" region="Intersect"/></example>
public static class RectExtensions {
  /// <summary>Returns the intersection of the specified rectangles.</summary>
  /// <param name="rect1">The first rectangle to compare.</param>
  /// <param name="rect2">The second rectangle to compare.</param>
  /// <returns>The intersection rectangle.</returns>
  /// <example><code source="Examples/Extensions/RectExtensions-Examples.cs" region="Intersect"/></example>
  public static Rect Intersect(this Rect rect1, Rect rect2) {
    var xMin = Mathf.Max(rect2.xMin, rect1.xMin);
    var xMax = Mathf.Min(rect2.xMax, rect1.xMax);
    var yMin = Mathf.Max(rect2.yMin, rect1.yMin);
    var yMax = Mathf.Min(rect2.yMax, rect1.yMax);
    return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
  }
}

}  // namespace
