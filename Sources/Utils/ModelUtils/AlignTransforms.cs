// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System.Linq;
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
  /// <see cref="Transform.up"/> direction of the source will be the opposite to the target.
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

  /// <summary>
  /// Aligns the vessel so that its node is located <i>against</i> the target's node. I.e. they are
  /// "looking" at the each other.
  /// </summary>
  /// <remarks>
  /// This method only does the positioning, and it ignores any physical properties. To avoid the
  /// physical consequences, the caller must take care of the physical differences (e.g. angular or
  /// linear speed).
  /// </remarks>
  /// <param name="vessel">The vessel to align.</param>
  /// <param name="vesselNode">The node at the vessel to align the target against.</param>
  /// <param name="targetNode">The node at the target to allign the vessel against.</param>
  /// <seealso cref="PlaceVessel"/>
  public static void SnapAlignVessel(Vessel vessel, Transform vesselNode, Transform targetNode) {
    var localChildRot = vessel.vesselTransform.rotation.Inverse() * vesselNode.rotation;
    vessel.SetRotation(
        Quaternion.LookRotation(-targetNode.forward, -targetNode.up) * localChildRot.Inverse());
    // The vessel position must be calculated and updated *after* the rotation is set, since it can
    // affect the vessel's node position.
    vessel.SetPosition(
        vessel.vesselTransform.position - (vesselNode.position - targetNode.position),
        usePristineCoords: true);
  }

  /// <summary>Aligns two vessels via the attach nodes.</summary>
  /// <remarks>
  /// The source vessel is positioned and rotated so that its attach node matches the target vessel
  /// attach node, and the nodes are "looking" at each other. 
  /// </remarks>
  /// <param name="srcAttachNode">The node of the source vessel.</param>
  /// <param name="tgtAttachNode">The node of the traget vessel.</param>
  public static void SnapAlignNodes(AttachNode srcAttachNode, AttachNode tgtAttachNode) {
    // The sequence of the calculations below is very order dependent! Don't change it.
    var srcVessel = srcAttachNode.owner.vessel;
    var srcNodeFwd = srcAttachNode.owner.transform.TransformDirection(srcAttachNode.orientation);
    var srcNodeRotation = Quaternion.LookRotation(srcNodeFwd);
    var localChildRot = srcVessel.vesselTransform.rotation.Inverse() * srcNodeRotation;
    var tgtNodeFwd = tgtAttachNode.owner.transform.TransformDirection(tgtAttachNode.orientation);
    var tgtNodePos = tgtAttachNode.owner.transform.TransformPoint(tgtAttachNode.position);
    srcVessel.SetRotation(Quaternion.LookRotation(-tgtNodeFwd) * localChildRot.Inverse());
    // The vessel position must be CALCULATED and UPDATED *after* the rotation is set, since it must
    // take into account the NEW vessel's rotation.
    var srcNodePos = srcAttachNode.owner.transform.TransformPoint(srcAttachNode.position);
    srcVessel.SetPosition(
        srcVessel.vesselTransform.position - (srcNodePos - tgtNodePos),
        usePristineCoords: true);
  }

  /// <summary>Places the vessel at the new position and resets the momentum on it.</summary>
  /// <remarks>If the vessel had any angular velocity, it will be reset to zero.</remarks>
  /// <param name="movingVessel">The vessel to place.</param>
  /// <param name="newPosition">The new position of the vessel.</param>
  /// <param name="newRotation">The new rotation of the vessel.</param>
  /// <param name="refVessel">
  /// The vessel to alignt the velocity with. If it's <c>null</c>, then the velocity on the moving
  /// vessel will just be zeroed.
  /// </param>
  public static void PlaceVessel(
      Vessel movingVessel, Vector3 newPosition, Quaternion newRotation, Vessel refVessel = null) {
    movingVessel.SetPosition(newPosition, usePristineCoords: true);
    movingVessel.SetRotation(newRotation);
    var refVelocity = Vector3.zero;
    if (refVessel != null) {
      refVelocity = refVessel.rootPart.Rigidbody.velocity;
    }
    foreach (var p in movingVessel.parts.Where(p => p.rb != null)) {
      p.rb.velocity = refVelocity;
      p.rb.angularVelocity = Vector3.zero;
    }
  }
}

}  // namespace
