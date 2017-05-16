// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KSPDev.ModelUtils {

/// <summary>Various tools to deal with game object hierarchy.</summary>
public static class Hierarchy {
  /// <summary>Regex to split an escaped path.</summary>
  static readonly Regex ReSplitEscapedPath = new Regex("(?<!/)/(?!/)");

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
  /// <param name="target">The target string to check.</param>
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
  /// <param name="parent">The transfrom to start from.</param>
  /// <param name="name">The name of the transfrom.</param>
  /// <returns>A transform or <c>null</c> if nothing is found.</returns>
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

  /// <summary>Finds a transform in the hirerachy by the provided path.</summary>
  /// <remarks>See path format in <see cref="FindTransformByPath(Transform,string[])"/>.</remarks>
  /// <param name="parent">The transfrom to start looking from.</param>
  /// <param name="path">
  /// The path to the target. The name components must be escaped if they contain the special
  /// symbols.
  /// </param>
  /// <returns>A transform or <c>null</c> if nothing found.</returns>
  /// <seealso cref="FindTransformByPath(Transform,string[])"/>
  /// <seealso cref="EscapeName"/>
  public static Transform FindTransformByPath(Transform parent, string path) {
    return FindTransformByPath(parent, SplitAndUnescapePath(path));
  }

  /// <summary>Finds a transform in the hirerachy by the provided path.</summary>
  /// <remarks>
  /// Every element of the path may specify an exact transform name or a partial match pattern:
  /// <list type="bullet">
  /// <item>
  /// <c>*</c> - any name matches. Such patterns can be nested to specify the desired level of
  /// nesting. E.g. <c>*/*/a</c> will look for name <c>a</c> in the grandchildren.
  /// </item>
  /// <item>
  /// <c>*</c> as a prefix - the name is matched by suffix. E.g. <c>*a</c> matches any name that
  /// ends with <c>a</c>.
  /// </item>
  /// <item>
  /// <c>*</c> as a suffix - the name is matched by prefix. E.g. <c>a*</c> matches any name that
  /// starts with <c>a</c>.
  /// </item>
  /// <item>
  /// <c>**</c> - any <i>path</i> matches. What will eventually be found depends on the pattern to
  /// the right of <c>**</c>. The path match pattern does a "breadth-first" search, i.e. it tries to
  /// find the shortest path possible. E.g. <c>**/a/b</c> will go through all the nodes starting
  /// from the parent until path <c>a/b</c> is found. Be careful with this pattern since in case of
  /// not matching anything it will walk thought the <i>whole</i> hirerachy.
  /// </item>
  /// </list>
  /// <para>
  /// All patterns except <c>**</c> may have a matching index. It can be used to resolve matches
  /// when there are multiple objects found with the same name and at the <i>same level</i>. E.g. if
  /// there are two objects with name "a" at the root level then the first one can be accessed by
  /// pattern <c>a:0</c>, and the second one by pattern <c>a:1</c>.
  /// </para>
  /// <para>
  /// Path search is <i>slow</i> since it needs walking though the hierarchy nodes. In the worst
  /// case all the nodes will be visited. Don't use this method in the performance demanding
  /// methods.
  /// </para>
  /// </remarks>
  /// <param name="parent">The transfrom to start looking from.</param>
  /// <param name="names">The path elements. All special symbols must be unescaped.</param>
  /// <returns>Transform or <c>null</c> if nothing found.</returns>
  /// <example>
  /// Given the following hierarchy:
  /// <code lang="c++"><![CDATA[
  /// // a
  /// // + b
  /// // | + c
  /// // | | + c1
  /// // | | + d
  /// // | + c
  /// // |   + d
  /// // |     + e
  /// // |       + e1
  /// // + abc
  /// ]]></code>
  /// <para>The following pattern/output will be possible:</para>
  /// <code><![CDATA[
  /// // a/b/c/d/e/e1 => node "e1"
  /// // a/b/*/d/e/e1 => node "e1"
  /// // a/b/*/*/e/e1 => node "e1"
  /// // a/b/c/c1 => node "с1"
  /// // a/b/*:0 => branch "a/b/c/c1", node "c"
  /// // a/b/*:1 => branch "a/b/c/d/e/e1", node "c"
  /// // a/b/c:1/d => branch "a/b/c/d/e/e1", node "d"
  /// // **/e1 => node "e1"
  /// // **/c1 => node "c1"
  /// // **/c/d => AMBIGUITY! The first found branch will be taken
  /// // a/**/e1 => node "e1"
  /// // *bc => node "abc"
  /// // ab* => node "abc"
  /// // *b* => node "abc"
  /// ]]></code>
  /// </example>
  /// <seealso cref="UnescapeName"/>
  public static Transform FindTransformByPath(Transform parent, string[] names) {
    if (names.Length == 0) {
      return parent;
    }
    // Try each child of the parent.
    var pair = names[0].Split(':');  // Separate index specifier if any.
    var pattern = pair[0];
    var reducedNames = names.Skip(1).ToArray();
    var index = pair.Length > 1 ? Math.Abs(int.Parse(pair[1])) : -1;
    for (var i = 0; i < parent.childCount; ++i) {
      var child = parent.GetChild(i);
      Transform branch = null;
      // "**" means "zero or more levels", so try parent's level first.
      if (pattern == "**") { 
        branch = FindTransformByPath(parent, reducedNames);
      }
      // Try all children treating "**" as "*" (one level).
      if (branch == null
          && (pattern == "*" || pattern == "**" || PatternMatch(pattern, child.name))) {
        if (index == -1 || index-- == 0) {
          branch = FindTransformByPath(child, reducedNames);
        }
      }
      if (branch != null) {
        return branch;
      }
    }

    // If "**" didn't match at this level the try it at the lover levels. For this just make a new
    // path "*/**" to go thru all the children and try "**" on them.
    if (pattern == "**") {
      var extendedNames = names.ToList();
      extendedNames.Insert(0, "*");
      return FindTransformByPath(parent, extendedNames.ToArray());
    }
    return null;
  }

  /// <summary>Returns part's model transform.</summary>
  /// <param name="part">The part to get model for.</param>
  /// <returns>
  /// The part's model transform if one was found. Or the root part's transform otherwise.
  /// </returns>
  public static Transform GetPartModelTransform(Part part) {
    var modelTransform = part.FindModelTransform("model");
    if (modelTransform == null) {
      Debug.LogErrorFormat("Cannot find model on part {0}", part.name);
      return part.transform;
    }
    return modelTransform;
  }

  /// <summary>
  /// Returns the paths to all the transformations in the object. Each item is a full path to the
  /// transformation starting from the <paramref name="parent"/>.
  /// </summary>
  /// <param name="parent">The object to start from.</param>
  /// <param name="pathPrefix">The prefix to add to every path in the result.</param>
  /// <returns>The paths to all the objects in the hirerachy separated by a LF symbol.</returns>
  public static string[] ListHirerahcy(Transform parent, string pathPrefix = "") {
    var res = new List<string>();
    GatherHirerachyNames(parent, pathPrefix, res);
    return res.ToArray();
  }

  /// <summary>Returns a full path to the object starting from the specified parent.</summary>
  /// <param name="obj">The object to find path for.</param>
  /// <param name="parent">
  /// The object at which the path must stop. If <c>null</c> then the path is gathered to the root
  /// object.
  /// </param>
  /// <returns>A full path name components. The names are not escaped.</returns>
  /// <seealso cref="FindTransformByPath(UnityEngine.Transform, string[])"/>
  public static string[] GetFullPath(Transform obj, Transform parent = null) {
    var path = new List<string>();
    while (obj != null && obj != parent) {
      path.Insert(0, obj.name);
      obj = obj.parent;
    }
    return path.ToArray();
  }

  /// <summary>Splits a path and unescapes the name elements.</summary>
  /// <remarks>
  /// The path elements must be separted by a <c>'/'</c> symbol. The name elements must be escaped
  /// in case of they contain a separator symbol in the content. 
  /// </remarks>
  /// <param name="escapedPath">The escaped path to split.</param>
  /// <returns>An array of unescaped names.</returns>
  /// <seealso cref="UnescapeName"/>
  public static string[] SplitAndUnescapePath(string escapedPath) {
    return ReSplitEscapedPath.Split(escapedPath).Select(v => UnescapeName(v)).ToArray();
  }

  /// <summary>Escapes the element names and builds a path.</summary>
  /// <param name="unescapedNames">The raw name elements of the path.</param>
  /// <returns>An escaped path built of the provided elements.</returns>
  /// <seealso cref="EscapeName"/>
  public static string MakePath(string[] unescapedNames) {
    return string.Join("/", unescapedNames.Select(v => EscapeName(v)).ToArray());
  }

  /// <summary>Unescapes all the special symbols in the name.</summary>
  /// <param name="escapedName">The name where all the special symbols are escaped.</param>
  /// <returns>An unescaped name.</returns>
  public static string UnescapeName(string escapedName) {
    return escapedName.Replace("//", "/");
  }

  /// <summary>Escapes all the special symbols in the name.</summary>
  /// <remarks>
  /// For now the only specil symbol is <c>/</c> (slash) because of it's used as a name separator
  /// in the hierarchy paths. The escaped separator symbol is simply doubled. I.e. string <c>a/b</c>
  /// transforms into <c>a//b</c>.
  /// </remarks>
  /// <param name="unescapedName">The raw name with any symbols.</param>
  /// <returns>A name where all the special symbols are properly escaped.</returns>
  public static string EscapeName(string unescapedName) {
    return unescapedName.Replace("/", "//");
  }

  #region Local helper methods.
  static void GatherHirerachyNames(Transform parent, string pathPrefix, List<string> names) {
    var fullName = pathPrefix != ""
        ? pathPrefix + "/" + EscapeName(parent.name)
        : EscapeName(parent.name);
    names.Add(fullName);
    for (var i = 0 ; i < parent.childCount; i++) {
      var child = parent.GetChild(i);
      GatherHirerachyNames(child, fullName, names);
    }
  }
  #endregion
}

}  // namespace
