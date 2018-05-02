// This is an intermediate module for methods and classes that are considred as candidates for
// KSPDev Utilities. Ideally, this module is always empty but there may be short period of time
// when new functionality lives here and not in KSPDev.

using KSPDev.ModelUtils;
using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.PartUtils {

/// <summary>Helper methods to deal with the part models.</summary>
public static class PartModel {
  /// <summary>Refreshes the highlighters on the part that owns the provided model.</summary>
  /// <remarks>
  /// When a part is highlighted (e.g. due to the mouse hover event), it highlights its models via a
  /// pre-cached set of the highlighter components. This cache is constructed on the part creation.
  /// If a model is added or removed from the part in runtime, the cache needs to be updated. This
  /// method does it by finding the part from the game objects hirerachy. If there is a part found,
  /// then its highlighters are updated.
  /// </remarks>
  /// <param name="modelObj">The game object which needs an update. It can be <c>null</c>.</param>
  public static void UpdateHighlighters(Transform modelObj) {
    if (modelObj == null) {
      return;
    }
    var ownerPart = modelObj.GetComponentInParent<Part>();
    if (ownerPart != null) {
      UpdateHighlighters(ownerPart);
    }
  }

  /// <summary>Refreshes the highlighters on the part.</summary>
  /// <remarks>
  /// It goes thru the highlighters cache and drops all the renderers that are no more in the part's
  /// model hierarchy. Then, it gets all the renderers in the hierarchy and enusrrs all of them are
  /// in the cache. It's not a cheap operation performance wise.
  /// </remarks>
  /// <param name="part">The part to refresh the highlighters for. It can be <c>null</c>.</param>
  public static void UpdateHighlighters(Part part) {
    if (part == null) {
      return;
    }
    if (part != null && part.HighlightRenderer != null) {
      var partModel = Hierarchy.GetPartModelTransform(part);
      // Drop the renderers that have left the part's model.
      part.HighlightRenderer.RemoveAll(x => x == null || !x.transform.IsChildOf(partModel));
      // Add the renderers that came into the part's model.
      part.HighlightRenderer.AddRange(
          part.GetComponentsInChildren<Renderer>()
              .Where(x => !part.HighlightRenderer.Contains(x)));
      part.RefreshHighlighter();
    }
  }
}
  
}  // namespace
