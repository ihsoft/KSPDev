// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.Types;
using System;
using UnityEngine;

namespace KSPDev.Extensions {

/// <summary>Helper extensions to deal with <see cref="PosAndRot"/> type.</summary>
/// <example>
/// <code source="Examples/Extensions/PosAndRotExtensions-Examples.cs" region="ToLocal"/>
/// <code source="Examples/Extensions/PosAndRotExtensions-Examples.cs" region="ToWorld"/>
/// </example>
public static class PosAndRotExtensions {
  /// <summary>
  /// Transforms a pos&amp;rot object from the world space to the local space. The opposite to
  /// <see cref="TransformPosAndRot"/>.
  /// </summary>
  /// <param name="node">The node to use as a parent.</param>
  /// <param name="posAndRot">The object in world space.</param>
  /// <returns>A new pos&amp;rot object in the local space.</returns>
  /// <example>
  /// <code source="Examples/Extensions/PosAndRotExtensions-Examples.cs" region="ToLocal"/>
  /// </example>
  public static PosAndRot InverseTransformPosAndRot(this Transform node, PosAndRot posAndRot) {
    return posAndRot.InverseTransform(node);
  }

  /// <summary>
  /// Transforms a pos&amp;rot object from the local space to the world space. The opposite to
  /// <see cref="InverseTransformPosAndRot"/>.
  /// </summary>
  /// <param name="node">The node to use as a parent.</param>
  /// <param name="posAndRot">The object in local space.</param>
  /// <returns>A new pos&amp;rot object in the wold space.</returns>
  /// <example>
  /// <code source="Examples/Extensions/PosAndRotExtensions-Examples.cs" region="ToWorld"/>
  /// </example>
  public static PosAndRot TransformPosAndRot(this Transform node, PosAndRot posAndRot) {
    return posAndRot.Transform(node);
  }
}

}  // namespace
