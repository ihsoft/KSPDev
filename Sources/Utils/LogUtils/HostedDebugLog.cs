// Kerbal Development tools.
// Author: igor.zavoychinskiy@gmail.com
// This software is distributed under Public domain license.

using UnityEngine;

namespace KSPDev.LogUtils {
  /// <summary>Helper class to log a record which is bound to a specific object.</summary>
  /// <remarks>
  /// It may be useful when there are situations that relate to a specific instance of the commong
  /// KSP objects. Like <see cref="Part"/>, <see cref="PartModule"/>, <see cref="Transform"/>, etc.
  /// With the hosted logging there will be no need to manually designate for which object the
  /// record is being logged.
  /// </remarks>
  /// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
  public static class HostedDebugLog {
    /// <summary>Logs a formatted INFO message with a host identifier.</summary>
    /// <remarks>
    /// For the <see cref="Part"/>, <see cref="PartModule"/> and <see cref="Transform"/> objects
    /// the identifier will be the most informative.
    /// </remarks>
    /// <param name="host">
    /// The host object which is bound to the log record. It can be <c>null</c>.
    /// </param>
    /// <param name="format">The format string for the log message.</param>
    /// <param name="args">The arguments for the format string.</param>
    /// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
    public static void Info(object host, string format, params object[] args) {
      Debug.logger.LogFormat(LogType.Log, GetHostName(host) + format, args);
    }

    /// <summary>Logs a formatted WARNING message with a host identifier.</summary>
    /// <inheritdoc cref="Info"/>
    /// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
    public static void Warning(object host, string format, params object[] args) {
      Debug.logger.LogFormat(LogType.Warning, GetHostName(host) + format, args);
    }

    /// <summary>Logs a formatted ERROR message with a host identifier.</summary>
    /// <inheritdoc cref="Info"/>
    /// <example><code source="Examples/LogUtils/HostedDebugLog-Examples.cs" region="HostedDebugLog1"/></example>
    public static void Error(object host, string format, params object[] args) {
      Debug.logger.LogFormat(LogType.Error, GetHostName(host) + format, args);
    }

    /// <summary>Helper method to make a user friendly host name for the logs.</summary>
    /// <param name="host">The host object that "owns" the logging. It can be <c>null</c>.</param>
    /// <returns>A human friendly string which identifies the host.</returns>
    static string GetHostName(object host) {
      var partHost = host as Part;
      if (partHost != null) {
        return "[Part:" + DbgFormatter.PartId(partHost) + "] ";
      }
      var moduleHost = host as PartModule;
      if (moduleHost != null) {
        var moduleNum = moduleHost.part.Modules.IndexOf(moduleHost);
        return "[Part:" + DbgFormatter.PartId(moduleHost.part) + "#Module:" + moduleNum + "] ";
      }
      var transformHost = host as Transform;
      if (transformHost != null) {
        return "[Tranform:" + DbgFormatter.TranformPath(transformHost) + "] ";
      }
      return "[Obj:" + (host != null ? host.ToString() : "NULL") + "] ";
    }
  }
}  // namespace
