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
}

}  // namespace
