// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.ModelUtils {

/// <summary>Helper methods to align transformations relative to each other.</summary>
public static class AlignTransforms {
  /// <summary>
  /// Aligns the source node so that it's located at the target, and source and target are "looking"
  /// at each other.
  /// </summary>
  /// <param name="source">Node to align.</param>
  /// <param name="sourceChild">Child node of the source to use as the align point.</param>
  /// <param name="target">Target node to align with.</param>
  public static void SnapAlign(Transform source, Transform sourceChild, Transform target) {
    source.rotation = target.rotation * source.rotation.Inverse() * sourceChild.rotation;
    source.position = source.position - (sourceChild.position - target.position);
  }
}

}  // namespace
