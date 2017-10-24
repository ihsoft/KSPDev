// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using System;
using System.Linq;
using UnityEngine;

namespace KSPDev.LogUtils {

/// <summary>Helper class to log a record which is bound to a specific object.</summary>
/// <remarks>
/// <para>
/// It may be useful when there are situations that relate to a specific instance of a common
/// KSP object. Like <see cref="Part"/>, <see cref="PartModule"/>, <see cref="Transform"/>, etc.
/// With the hosted logging, there will be no need to manually designate for which object the
/// record is being logged.
/// </para>
/// <para>
/// Another benefit of this logging class is that it can better resolve the arguments of the certain
/// types. E.g. when logging out a value referring a <see cref="Transform"/> type, the resulted
/// record will represent a full hierrachy path instead of just the object name. See
/// <see cref="DebugEx.ObjectToString"/> for the full list of the supported types.
/// </para>
/// </remarks>
/// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
/// <seealso cref="DebugEx"/>
public static class HostedDebugLog {
  /// <summary>Logs a formatted INFO message with a host identifier.</summary>
  /// <param name="host">
  /// The host object which is bound to the log record. It can be <c>null</c>.
  /// </param>
  /// <param name="format">The format string for the log message.</param>
  /// <param name="args">The arguments for the format string.</param>
  /// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
  /// <seealso cref="DebugEx.ObjectToString"/>
  /// <seealso cref="Log"/>
  public static void Info(Part host, string format, params object[] args) {
    Log(LogType.Log, host, format, args);
  }

  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Info(PartModule host, string format, params object[] args) {
    Log(LogType.Log, host, format, args);
  }

  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Info(Transform host, string format, params object[] args) {
    Log(LogType.Log, host, format, args);
  }

  /// <summary>
  /// Logs a formatted INFO message with a host identifier when the <i>verbose</i> logging mode is
  /// enabled.
  /// </summary>
  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Fine(Part host, string format, params object[] args) {
    if (GameSettings.VERBOSE_DEBUG_LOG) {
      Log(LogType.Log, host, format, args);
    }
  }

  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Fine(PartModule host, string format, params object[] args) {
    if (GameSettings.VERBOSE_DEBUG_LOG) {
      Log(LogType.Log, host, format, args);
    }
  }

  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Fine(Transform host, string format, params object[] args) {
    if (GameSettings.VERBOSE_DEBUG_LOG) {
      Log(LogType.Log, host, format, args);
    }
  }

  /// <summary>Logs a formatted WARNING message with a host identifier.</summary>
  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Warning(Part host, string format, params object[] args) {
    Log(LogType.Warning, host, format, args);
  }

  /// <inheritdoc cref="Warning(Part,string,object[])"/>
  public static void Warning(PartModule host, string format, params object[] args) {
    Log(LogType.Warning, host, format, args);
  }

  /// <inheritdoc cref="Warning(Part,string,object[])"/>
  public static void Warning(Transform host, string format, params object[] args) {
    Log(LogType.Warning, host, format, args);
  }

  /// <summary>Logs a formatted ERROR message with a host identifier.</summary>
  /// <inheritdoc cref="Info(Part,string,object[])"/>
  public static void Error(Part host, string format, params object[] args) {
    Log(LogType.Error, host, format, args);
  }

  /// <inheritdoc cref="Error(Part,string,object[])"/>
  public static void Error(PartModule host, string format, params object[] args) {
    Log(LogType.Error, host, format, args);
  }

  /// <inheritdoc cref="Error(Part,string,object[])"/>
  public static void Error(Transform host, string format, params object[] args) {
    Log(LogType.Error, host, format, args);
  }

  /// <summary>Generic method to emit a hosted log record.</summary>
  /// <param name="type">The type of the log record.</param>
  /// <param name="host">
  /// The host object which is bound to the log record. It can be <c>null</c>.
  /// </param>
  /// <param name="format">The format string for the log message.</param>
  /// <param name="args">The arguments for the format string.</param>
  /// <seealso cref="DebugEx.ObjectToString"/>
  public static void Log(LogType type, object host, string format, params object[] args) {
    DebugEx.Log(type, DebugEx.ObjectToString(host) + " " + format, args);
  }
}

}  // namespace
