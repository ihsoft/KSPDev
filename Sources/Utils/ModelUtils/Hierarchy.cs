// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KSPDev.ModelUtils {

/// <summary>Various tools to deal with game object hierarchy.</summary>
public static class Hierarchy {
  /// <summary>Changes transform's parent keeping local postion, rotation and scale.</summary>
  /// <remarks>
  /// Normally, Unity preserves world position, rotation and scale when changing parent. It's
  /// convinient when managing objects in a prefab but is not desired when constructing a new model.
  /// </remarks>
  /// <param name="child">Transform to change parent for.</param>
  /// <param name="parent">Transform to change parent to.</param>
  /// <param name="newPosition">Local position to set instead of the original one.</param>
  /// <param name="newRotation">Local rotation to set instead of the original one.</param>
  /// <param name="newScale">Local scale to set instead of the original one.</param>
  public static void MoveToParent(Transform child, Transform parent,
                                  Vector3? newPosition = null,
                                  Quaternion? newRotation = null,
                                  Vector3? newScale = null) {
    var position = newPosition ?? child.localPosition;
    var rotation = newRotation ?? child.localRotation;
    var scale = newScale ?? child.localScale;
    child.parent = parent;
    child.localPosition = position;
    child.localRotation = rotation;
    child.localScale = scale;
  }

  /// <summary>
  /// Checks target string against a simple pattern which allows prefix, suffix, and contains match.
  /// The match is case-sensitive.
  /// </summary>
  /// <param name="pattern">
  /// Pattern to match for:
  /// <list type="bullet">
  /// <item>If pattern ends with <c>*</c> then it's a match by prefix.</item>
  /// <item>If pattern starts with <c>*</c> then it's a match by suffix.</item>
  /// <item>
  /// If pattern starts and ends with <c>*</c> then pattern is searched anywhere in the target.
  /// </item>
  /// </list>
  /// </param>
  /// <param name="target">Target string to check.</param>
  /// <returns><c>true</c> if pattern matches the target.</returns>
  public static bool PatternMatch(string pattern, string target) {
    if (pattern.Length > 0 && pattern[0] == '*') {
      return target.EndsWith(pattern.Substring(1), StringComparison.Ordinal);
    }
    if (pattern.Length > 0 && pattern[pattern.Length - 1] == '*') {
      return target.StartsWith(pattern.Substring(0, pattern.Length - 1), StringComparison.Ordinal);
    }
    if (pattern.Length > 1 && pattern[0] == '*' && pattern[pattern.Length - 1] == '*') {
      return target.Contains(pattern.Substring(1, pattern.Length - 2));
    }
    return target == pattern;
  }

  /// <summary>Finds a transform by name down the hierarchy.</summary>
  /// <remarks>
  /// Implements breadth-first search approach to minimize depth of the found transform.
  /// </remarks>
  /// <param name="parent">Transfrom to start from.</param>
  /// <param name="name">Name of the transfrom.</param>
  /// <returns>Found transform or <c>null</c> if nothing is found.</returns>
  public static Transform FindTransformInChildren(Transform parent, string name) {
    var res = parent.Find(name);
    if (res != null) {
      return res;
    }
    for (var i = parent.childCount - 1; i >= 0; --i) {
      res = FindTransformInChildren(parent.GetChild(i), name);
      if (res != null) {
        return res;
      }
    }
    return null;
  }

  /// <summary>Finds transform treating the name as a hierarchy path.</summary>
  /// <param name="parent">Transfrom to start looking from.</param>
  /// <param name="path">
  /// Path to the target. Path elements are separated by "/" symbol. Element can be name or a
  /// pattern.
  /// </param>
  /// <returns>Transform or <c>null</c> if nothing found.</returns>
  public static Transform FindTransformByPath(Transform parent, string path) {
    return FindTransformByPath(parent, path.Split('/'));
  }

  /// <summary>Finds transform treating the name as a hierarchy path.</summary>
  /// <remarks>
  /// Elements of the path may specify exact transform name or be one of the following patterns:
  /// <list type="bullet">
  /// <item>
  /// "*" - any child  will match. I.e. all children of the preceding parent will be checked for the
  /// branch that follows the pattern. First full match will be returned. E.g. if the are parts
  /// "a/b/c" and "a/aa/c" then pattern "a/*/c" will match "a/b/c" since child "b" is the first in
  /// the children list. This pattern can be nested to specify that barcnh is expected to be found
  /// at the exact depth: "a/*/*/c".
  /// </item>
  /// <item>
  /// "**" - any path will match. I.e. all the branches of the preceding parent will be checked
  /// until one of them is matched the sub-path that follows the pattern. The shortest path is used
  /// in case of multple hits. E.g. if there are paths "a/b/c" and "a/c" then pattern "a/**/c" will
  /// match path "a/c". This pattern cannot be followed by another pattern, but it can follow "*"
  /// pattern, e.g. "a/*/**/c" (get "c" from any branch of "a" given the depth level is greater than
  /// 1).
  /// </item>
  /// </list>
  /// <para>
  /// Keep in mind that patterns require children scan, and in a worst case scenario all the
  /// hirerachy can be scanned multiple times.
  /// </para>
  /// </remarks>
  /// <param name="parent">Transfrom to start looking from.</param>
  /// <param name="names">Path elements.</param>
  /// <returns>Transform or <c>null</c> if nothing found.</returns>
  public static Transform FindTransformByPath(Transform parent, string[] names) {
    if (names.Length == 0) {
      return parent;
    }
    var name = names[0];
    names = names.Skip(1).ToArray();
    for (var i = parent.childCount - 1; i >= 0; --i) {
      var child = parent.GetChild(i);
      if (name == "*") {
        var branch = FindTransformByPath(child, names); // slice
        if (branch != null) {
          return branch;
        }
      } else if (name == "**" && child.name == names[0] || child.name == name) {
        if (name == "**") {
          names = names.Skip(1).ToArray();
        }
        return FindTransformByPath(child, names);
      }
    }
    // If "**" pattern is not found in the children then go thru children branches.
    if (name == "**") {
      var nextName = names[0];
      var nextNames = names.Skip(1).ToArray();
      return FindTransformInChildren(parent, nextName);
    }
    return null;
  }

  /// <summary>Returns part's model transform.</summary>
  /// <param name="part">Part to get model for.</param>
  /// <returns>PartModel's transform if it was found. Part's trasnfrom otherwise.</returns>
  public static Transform GetPartModelTransform(Part part) {
    var modelTransform = part.FindModelTransform("model");
    if (modelTransform == null) {
      Debug.LogErrorFormat("Cannot find model on part {0}", part.name);
      return part.transform;
    }
    return modelTransform;
  }

  /// <summary>
  /// Returns paths to all transformations in the object. Each item is a full path to the
  /// transformation.
  /// </summary>
  /// <param name="parent">Object to start from.</param>
  /// <param name="parentPath">
  /// Path to <paramref name="parent"/>. Leave it <c>null</c> if paths should start from "/".
  /// </param>
  /// <returns>paths to all transformations separated by LF symbol.</returns>
  public static string[] ListHirerahcy(Transform parent, string parentPath = null) {
    var res = new List<string>();
    GatherHirerachyNames(parent, parentPath, res);
    return res.ToArray();
  }

  /// <summary>Returns full path to the object starting from the specified parent.</summary>
  /// <param name="obj">Object to find path for.</param>
  /// <param name="parent">Optional parent to use a root.</param>
  /// <returns>Full path to the object.</returns>
  /// <seealso cref="FindTransformByPath(UnityEngine.Transform, string[])"/>
  public static string[] GetFullPath(Transform obj, Transform parent = null) {
    var path = new List<string>();
    while (obj != null && obj != parent) {
      path.Insert(0, obj.name);
      obj = obj.parent;
    }
    return path.ToArray();
  }

  #region Local helper methods.
  static void GatherHirerachyNames(Transform parent, string parentPath, List<string> names) {
    var fullName = (parentPath ?? "/") + parent.name + "/";
    names.Add(fullName);
    for (var i = 0 ; i < parent.childCount; i++) {
      var child = parent.GetChild(i);
      GatherHirerachyNames(child, fullName, names);
    }
  }
  #endregion
}

}  // namespace
