// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using KSPDev.ModelUtils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KSPDev.LogUtils {

/// <summary>A set of tools to format various game enities for debugging purposes.</summary>
public static class DbgFormatter {
  /// <summary>Returns a user friendly unique description of the part.</summary>
  /// <param name="p">Part to get ID string for.</param>
  /// <returns>ID string.</returns>
  public static string PartId(Part p) {
    return p != null
        ? string.Format("{0} (id={1})", p.name, p.flightID)
        : "Part#NULL";
  }

  /// <summary>Returns a string represenation of a vector with more precision.</summary>
  /// <param name="vec">Vector to dump.</param>
  /// <returns>String representation.</returns>
  public static string Vector(Vector3 vec) {
    return string.Format("({0:0.0###}, {1:0.0###}, {2:0.0###})", vec.x, vec.y, vec.z);
  }

  /// <summary>Returns a string represenation of a quaternion with more precision.</summary>
  /// <param name="rot">Quaternion to dump.</param>
  /// <returns>String representation.</returns>
  public static string Quaternion(Quaternion rot) {
    return string.Format("({0:0.0###}, {1:0.0###}, {2:0.0###}, {3:0.0###})",
                         rot.x, rot.y, rot.z, rot.w);
  }

  /// <summary>Returns a full string path for the tranform.</summary>
  /// <param name="obj">Object to make the path for.</param>
  /// <param name="parent">Optional parent to use a root.</param>
  /// <returns>Full string path to the root.</returns>
  public static string TranformPath(Transform obj, Transform parent = null) {
    return obj != null 
        ? Hierarchy.MakePath(Hierarchy.GetFullPath(obj, parent))
        : "Transform#NULL";
  }

  /// <summary>Returns a full string path for the game object.</summary>
  /// <param name="obj">Object to make the path for.</param>
  /// <param name="parent">Optional parent to use a root.</param>
  /// <returns>Full string path to the root.</returns>
  public static string TranformPath(GameObject obj, Transform parent = null) {
    return obj != null 
        ? TranformPath(obj.transform, parent)
        : "GameObject#NULL";
  }

  /// <summary>Flatterns collection items into a comma separated string.</summary>
  /// <remarks>This method's name is a shorthand for "Collection-To-String". Given a collection
  /// (e.g. list, set, or anything else implementing <c>IEnumarable</c>) this method transforms it
  /// into a human readable string.</remarks>
  /// <param name="collection">A collection to represent as a string.</param>
  /// <param name="predicate">A predicate to use to extract string representation of an item. If
  /// <c>null</c> then standard <c>ToString()</c> is used.</param>
  /// <param name="separator">String to use to glue the parts.</param>
  /// <returns>Human readable form of the collection.</returns>
  /// <typeparam name="TSource">Collection's item type.</typeparam>
  public static string C2S<TSource>(IEnumerable<TSource> collection,
                                    Func<TSource, string> predicate = null,
                                    string separator = ",") {
    if (collection == null) {
      return "Collection#NULL";
    }
    var res = new StringBuilder();
    var firstItem = true;
    foreach (var item in collection) {
      if (firstItem) {
        firstItem = false;
      } else {
        res.Append(separator);
      }
      if (predicate != null) {
        res.Append(predicate(item));
      } else {
        res.Append(item.ToString());
      }
    }
    return res.ToString();
  }
}

}  // namespace
