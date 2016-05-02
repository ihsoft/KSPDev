// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Collections.Generic;
using System.Text;

namespace KSPDev.LogUtils {

/// <summary>A set of convenience logging methods.</summary>
public static class Logger {
  /// <summary>Logs a formatted message as INFO record.</summary>
  /// <seealso cref="string.Format(string, object)"/>
  /// <param name="fmt">A standard C# format string.</param>
  /// <param name="args">Arguments for the format string.</param>
  public static void logInfo(String fmt, params object[] args) {
    UnityEngine.Debug.Log(String.Format(fmt, args));
  }

  /// <summary>Logs a formatted message as WARNING record.</summary>
  /// <seealso cref="string.Format(string, object)"/>
  /// <param name="fmt">A standard C# format string.</param>
  /// <param name="args">Arguments for the format string.</param>
  public static void logWarning(String fmt, params object[] args) {
    UnityEngine.Debug.LogWarning(String.Format(fmt, args));
  }

  /// <summary>Logs a formatted message as ERROR record.</summary>
  /// <seealso cref="string.Format(string, object)"/>
  /// <param name="fmt">A standard C# format string.</param>
  /// <param name="args">Arguments for the format string.</param>
  public static void logError(String fmt, params object[] args) {
    UnityEngine.Debug.LogError(String.Format(fmt, args));
  }

  /// <summary>Logs an exception stack trace as EXCEPTION record.</summary>
  /// <param name="ex">An exception to log.</param>
  public static void logException(Exception ex) {
    UnityEngine.Debug.LogException(ex);
  }

  /// <summary>Flatterns collection items into a comma separated string.</summary>
  /// <remarks>This method's name is a shorthand for "Collection-To-String". Given a collection
  /// (e.g. list, set, or anything else implementing <c>IEnumarable</c>) this method transforms it
  /// into a human readable string.</remarks>
  /// <param name="collection">A collection to represent as a string.</param>
  /// <param name="predicate">A predicate to use to extract string representation of an item. If
  /// <c>null</c> then standard <c>ToString()</c> is used.</param>
  /// <returns>Human readable form of the collection.</returns>
  public static String C2S<TSource>(
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
