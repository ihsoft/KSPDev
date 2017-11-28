// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.ModelUtils {

/// <summary>Helper methods to align transformations relative to each other.</summary>
public static class AlignTransforms {
  /// <summary>
  /// Aligns the source node so that it's located at the target, and the source and target are
  /// "looking" at the each other.
  /// </summary>
  /// <remarks>
  /// The object's "look" direction is a <see cref="Transform.forward"/> direction. The resulted
  /// <see cref="Transform.up"/> direction of the source will be the same as on the target.
  /// </remarks>
  /// <param name="source">The node to align.</param>
  /// <param name="sourceChild">The child node of the source to use as the align point.</param>
  /// <param name="target">The target node to align with.</param>
  /// <include file="Unity3D_HelpIndex.xml" path="//item[@name='T:UnityEngine.Transform']/*"/>
  public static void SnapAlign(Transform source, Transform sourceChild, Transform target) {
    // Don't relay on the localRotation since the child may be not an immediate child.
    var localChildRot = source.rotation.Inverse() * sourceChild.rotation;
    source.rotation =
        Quaternion.LookRotation(-target.forward, -target.up) * localChildRot.Inverse();
    source.position = source.position - (sourceChild.position - target.position);
  }
}

}  // namespace
