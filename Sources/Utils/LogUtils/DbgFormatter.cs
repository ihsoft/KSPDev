// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Text;

namespace KSPDev.LogUtils {

/// <summary>A set of tools to format various game enities for debugging purposes.</summary>
public static class DbgFormatter {
  /// <summary>Returns a user friendly unique description of the part.</summary>
  /// <param name="p">Part to get ID string for.</param>
  /// <returns>ID string.</returns>
  public static string PartId(Part p) {
    return p != null ? string.Format("{0} (id={1})", p.name, p.flightID) : "NULL";
  }

  /// <summary>Flatterns collection items into a comma separated string.</summary>
  /// <remarks>This method's name is a shorthand for "Collection-To-String". Given a collection
  /// (e.g. list, set, or anything else implementing <c>IEnumarable</c>) this method transforms it
  /// into a human readable string.</remarks>
  /// <param name="collection">A collection to represent as a string.</param>
  /// <param name="predicate">A predicate to use to extract string representation of an item. If
  /// <c>null</c> then standard <c>ToString()</c> is used.</param>
  /// <returns>Human readable form of the collection.</returns>
  /// <typeparam name="TSource">Collection's item type.</typeparam>
  public static string C2S<TSource>(
      IEnumerable<TSource> collection, Func<TSource, string> predicate = null) {
    var res = new StringBuilder();
    var firstItem = true;
    foreach (var item in collection) {
      if (firstItem) {
        firstItem = false;
      } else {
        res.Append(',');
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
