// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;

namespace KSPDev {

/// <summary>
/// A development logger that respects logging level and supports string formatting.
/// </summary>
public static class Logger {
  public static void logInfo(String fmt, params object[] args) {
    UnityEngine.Debug.Log(String.Format(fmt, args));
  }

  public static void logWarning(String fmt, params object[] args) {
    UnityEngine.Debug.LogWarning(String.Format(fmt, args));
  }

  public static void logError(String fmt, params object[] args) {
    UnityEngine.Debug.LogError(String.Format(fmt, args));
  }

  public static void logException(Exception ex) {
    UnityEngine.Debug.LogException(ex);
  }
}

}  // namespace
